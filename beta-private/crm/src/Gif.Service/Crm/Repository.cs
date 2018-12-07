#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using Samc4.CipherUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Gif.Service.Enums;

namespace Gif.Service.Crm
{
    public class Repository : IRepository
    {
        #region Fields

        protected readonly AuthenticationContext _authContext;

        protected readonly AuthenticationResult _authResult;

        private IConfiguration config;

        #endregion

        #region Properties

        public virtual string CrmUrl
        {
            get
            {
                return config["CrmUrl"];
            }
        }

        #endregion

        private const string jsonSeparator = "\":\"";
        private const string doubleQuote = "\"";

        #region Constructors

        public Repository()
        {
            config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var secret = CipherUtil.Decrypt<AesManaged>(config["EncryptedClientSecret"], "GifService", config["AzureClientId"]);
            _authContext = new AuthenticationContext(config["CrmAuthority"], false);
            _authResult = _authContext.AcquireTokenAsync(config["CrmUrl"], new ClientCredential(config["AzureClientId"], secret)).Result; //_authContext.AcquireToken() (ConfigurationManager.AppSettings["CrmUrl"], ConfigurationManager.AppSettings["CrmClientId"], credentials);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        #endregion

        #region Private Methods

        private HttpClient getCrmConnection()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(config["CrmUrl"] + "/api/data/v9.0/"),
                Timeout = new TimeSpan(0, 2, 0)
            };
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=OData.Community.Display.V1.FormattedValue");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
            return httpClient;
        }

        #endregion

        #region Virtual Methods

        public virtual JObject Retrieve(string query)
        {
            HttpResponseMessage httpResponse;

            using (var httpClient = getCrmConnection())
            {
                httpResponse = httpClient.GetAsync(query).Result;
            }

            JObject jretrieveJObject = null;

            if (httpResponse.IsSuccessStatusCode)
            {
                jretrieveJObject = JObject.Parse(httpResponse.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new CrmApiException(httpResponse.ReasonPhrase, httpResponse.StatusCode);
            }

            return jretrieveJObject;
        }

        public virtual JToken RetrieveMultiple(string query, out int? count)
        {
            JToken jretrieveToken = null;
            count = null;

            HttpResponseMessage retrieveResponse;

            using (var httpClient = getCrmConnection())
            {
                retrieveResponse = httpClient.GetAsync(query).Result;
            }

            if (retrieveResponse.StatusCode != HttpStatusCode.OK && retrieveResponse.StatusCode != HttpStatusCode.NoContent)
                throw new CrmApiException(retrieveResponse.ReasonPhrase, retrieveResponse.StatusCode);

            var jretrieveJObject = JObject.Parse(retrieveResponse.Content.ReadAsStringAsync().Result);

            if (jretrieveJObject == null)
                return jretrieveToken;

            jretrieveToken = jretrieveJObject["value"];

            if (jretrieveJObject["@odata.count"] != null)
            {
                count = int.Parse(jretrieveJObject["@odata.count"].ToString());
            }

            return jretrieveToken;
        }

        public virtual void Associate(Guid entityId1, string entityName1, Guid entityId2, string entityName2, string relationshipKey)
        {
            HttpResponseMessage resp;

            using (var httpClient = getCrmConnection())
            {
                var address = $"{entityName1}({entityId1.ToString()})/{relationshipKey}/$ref";

                var associated = $"{{\"@odata.id\":\"{httpClient.BaseAddress}{entityName2}({entityId2})\"}}";

                var content = new StringContent(associated, Encoding.UTF8, "application/json");

                resp = httpClient.PostAsync(address, content).Result;
            }

            if (resp.StatusCode != HttpStatusCode.OK && resp.StatusCode != HttpStatusCode.NoContent)
                throw new CrmApiException(resp.ReasonPhrase, resp.StatusCode);

        }

        public virtual void UpdateField(string entityName, string entityField, Guid entityId, string value)
        {
            var address = entityName + "(" + entityId + ")/" + entityField;

            //Don't add quotes if value is a bool/int
            bool boolField; int intField; string updateBody;
            if (bool.TryParse(value, out boolField) || int.TryParse(value, out intField))
            {
                updateBody = "{\"value\": " + value.ToLower() + "}";
            }
            else
            {
                updateBody = "{\"value\":\"" + value.ToLower() + "\"}";
            }

            var content = new StringContent(updateBody, Encoding.UTF8, "application/json");

            HttpResponseMessage updateResponse;

            using (var httpClient = getCrmConnection())
            {
                updateResponse = httpClient.PutAsync(address, content).Result;
            }

            if (updateResponse.StatusCode != HttpStatusCode.NoContent)
                throw new CrmApiException(updateResponse.ReasonPhrase, updateResponse.StatusCode);
        }

        /// <summary>
        /// Create a new CRM Entity
        /// </summary>
        /// <param name="entityName">The name of the new entity to create</param>
        /// <param name="entityData">A JSON formatted string of the entity fields to be populated and the values to enter</param>
        /// <param name="update"></param>
        /// <returns>True or throws an error to be caught.</returns>
        public virtual Guid CreateEntity(string entityName, string entityData, bool update = false)
        {
            var address = entityName;
            var content = new StringContent(entityData, Encoding.UTF8, "application/json");

            HttpResponseMessage updateResponse;
            string targetUri;

            using (var httpClient = getCrmConnection())
            {
                updateResponse = httpClient.PostAsync(address, content).Result;
                targetUri = httpClient.BaseAddress.AbsoluteUri;
            }

            if (updateResponse.StatusCode != HttpStatusCode.OK && updateResponse.StatusCode != HttpStatusCode.NoContent)
                throw new CrmApiException(updateResponse.ReasonPhrase, updateResponse.StatusCode);

            IEnumerable<string> headerVals;

            if (!updateResponse.Headers.TryGetValues("OData-EntityId", out headerVals))
            {
                throw new FormatException("Response Entity ID header is empty");
            }

            var idString = new List<string>(headerVals)[0].Replace(targetUri + entityName, "");

            return Guid.Parse(idString);
        }

        public virtual void UpdateEntity(string entityName, Guid id, string entityData)
        {
            HttpResponseMessage response;
            var method = new HttpMethod("PATCH");
            var content = new StringContent(entityData, Encoding.UTF8, "application/json");

            using (var httpClient = getCrmConnection())
            {
                var requestUri = new Uri($"{httpClient.BaseAddress.AbsoluteUri}{entityName}({id})");

                var request = new HttpRequestMessage(method, requestUri)
                {
                    Content = content
                };

                response = httpClient.SendAsync(request).Result;
            }

            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new CrmApiException(response.ReasonPhrase, response.StatusCode);

        }

        public virtual void Delete(string entityName, Guid id)
        {
            HttpResponseMessage response;
            var method = new HttpMethod("DELETE");

            using (var httpClient = getCrmConnection())
            {
                var requestUri = new Uri($"{httpClient.BaseAddress.AbsoluteUri}{entityName}({id})");

                var request = new HttpRequestMessage(method, requestUri);
                response = httpClient.SendAsync(request).Result;
            }

            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new CrmApiException(response.ReasonPhrase, response.StatusCode);
        }

        public void CreateBatch(List<BatchData> batchData)
        {
            HttpResponseMessage response;

            using (var httpClient = getCrmConnection())
            {
                // batch setup
                var batchId = Guid.NewGuid();
                var deleteChangeId = Guid.NewGuid();
                var patchChangeId = Guid.NewGuid();
                var batchUrl = new Uri($"{httpClient.BaseAddress.AbsoluteUri}$batch");

                var batchRequest = new HttpRequestMessage(HttpMethod.Post, batchUrl);
                var batchContent = new MultipartContent("mixed", "batch_" + batchId);

                // changeset setup
                var deleteChange = new MultipartContent("mixed", "changeset_" + deleteChangeId);
                var patchChange = new MultipartContent("mixed", "changeset_" + patchChangeId);

                //Add deletes first in their own changeset to avoid issues with upserts in following changesets
                AddChangeSet(batchData.Where(x => x.Type == BatchTypeEnum.Delete).ToList(), httpClient, ref deleteChange);
                AddChangeSet(batchData.Where(x => x.Type != BatchTypeEnum.Delete).ToList(), httpClient, ref patchChange);

                //Add headers add changeset level
                AddHeadersToChangeSets(ref deleteChange);
                AddHeadersToChangeSets(ref patchChange);

                // Add the changesets to the batch content
                batchContent.Add(deleteChange);
                batchContent.Add(patchChange);

                // send batch
                batchRequest.Content = batchContent;
                response = httpClient.SendAsync(batchRequest).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new CrmApiException(response.ReasonPhrase, response.StatusCode);
            }

        }


        private void AddChangeSet(IList<BatchData> batchData, HttpClient httpClient, ref MultipartContent changeSet)
        {
            var count = 1;

            foreach (var batch in batchData)
            {
                var content = new StringContent(batch.EntityData, Encoding.UTF8, "application/json");
                var requestUri = new Uri($"{httpClient.BaseAddress.AbsoluteUri}{batch.Name}({batch.Id})");
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "application/json;type=entry");
                content.Headers.Add("Content-Transfer-Encoding", "binary");
                content.Headers.Add("Content-Id", count.ToString());

                var method = batch.Type == BatchTypeEnum.Delete ?
                    new HttpMethod("DELETE") : new HttpMethod("PATCH");

                var request = new HttpRequestMessage(method, requestUri)
                {
                    Content = content
                };

                // Add this content to the changeset
                changeSet.Add(new HttpMessageContent(request));
                count++;
            }
        }
        public void AddHeadersToChangeSets(ref MultipartContent content)
        {
            using (var enumChangeSet = content.GetEnumerator())
            {
                while (enumChangeSet.MoveNext())
                {
                    var currentChangeSet = enumChangeSet.Current;
                    currentChangeSet.Headers.ContentType = new MediaTypeHeaderValue("application/http");
                    currentChangeSet.Headers.Add("Content-Transfer-Encoding", "binary");
                }
            }
        }

        #endregion

    }

}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
