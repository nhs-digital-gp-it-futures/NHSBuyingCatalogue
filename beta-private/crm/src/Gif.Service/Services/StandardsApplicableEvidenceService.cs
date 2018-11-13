#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Attributes;
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gif.Service.Services
{
    public class StandardsApplicableEvidenceService : ServiceBase, IStandardsApplicableEvidenceDatastore
    {
        public StandardsApplicableEvidenceService(IRepository repository) : base(repository)
        {
        }

        public IEnumerable<Evidence> ByClaim(string standardApplicableId)
        {
            var evidences = new List<Evidence>();

            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("StandardApplicableId") {FilterName = "_cc_standardapplicableid_value", FilterValue = standardApplicableId},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new Evidence().GetQueryString(null, filterAttributes, true, true), out Count);

            foreach (var evidence in appJson.Children())
            {
                evidences.Add(new Evidence(evidence));
            }

            var enumEvidences = OrderLinkedEvidences(evidences);

            Count = evidences.Count;

            return enumEvidences;
        }

        public Evidence ById(string id)
        {
            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("EvidenceId") {FilterName = "cc_evidenceid", FilterValue = id},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new Evidence().GetQueryString(null, filterAttributes), out Count);
            var evidence = appJson?.FirstOrDefault();

            return new Evidence(evidence);
        }

        public Evidence Create(Evidence evidence)
        {
            Repository.CreateEntity(evidence.EntityName, evidence.SerializeToODataPost());

            return evidence;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
