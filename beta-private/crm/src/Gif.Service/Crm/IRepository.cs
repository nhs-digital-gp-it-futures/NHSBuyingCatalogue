#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Newtonsoft.Json.Linq;
using System;

namespace Gif.Service.Crm
{
    public interface IRepository
    {
        string CrmUrl { get; }
        JObject Retrieve(string query);
        JToken RetrieveMultiple(string query, out int? count);
        void Associate(Guid entityId1, string entName1, Guid entityId2, string entName2, string relationshipKey);
        void UpdateField(string entityName, string entityField, Guid entityId, string value);
        Guid CreateEntity(string entityName, string entityData, bool update = false);
        void UpdateEntity(string entityName, Guid id, string entityData);
        void Delete(string entityName, Guid id);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
