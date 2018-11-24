#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Attributes;
using Gif.Service.Const;
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

        public IEnumerable<StandardApplicableEvidence> ByClaim(string standardApplicableId)
        {
            var evidences = new List<StandardApplicableEvidence>();

            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("StandardApplicableId") {FilterName = "_cc_standardapplicable_value", FilterValue = standardApplicableId},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new StandardApplicableEvidence().GetQueryString(null, filterAttributes, true, true), out Count);

            foreach (var evidence in appJson.Children())
            {
                evidences.Add(new StandardApplicableEvidence(evidence));
            }

            var enumEvidences = StandardApplicableEvidence.OrderLinkedEvidences(evidences);

            Count = evidences.Count;

            return enumEvidences;
        }

        IEnumerable<IEnumerable<StandardApplicableEvidence>> IEvidenceDatastore<StandardApplicableEvidence>.ByClaim(string claimId)
        {
            throw new System.NotImplementedException();
        }

        public StandardApplicableEvidence ById(string id)
        {
            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("EvidenceId") {FilterName = "cc_evidenceid", FilterValue = id},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new StandardApplicableEvidence().GetQueryString(null, filterAttributes), out Count);
            var evidence = appJson?.FirstOrDefault();

            return new StandardApplicableEvidence(evidence);
        }

        public StandardApplicableEvidence Create(StandardApplicableEvidence evidenceEntity)
        {
            Repository.CreateEntity(evidenceEntity.EntityName, evidenceEntity.SerializeToODataPost());

            return evidenceEntity;
        }

        public StandardApplicable ByEvidenceId(string id)
        {
            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("EvidenceId") {FilterName = "cc_evidenceid", FilterValue = id},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new StandardApplicableEvidence().GetQueryString(null, filterAttributes, true), out Count);
            var standardApplicable = appJson?.Children().FirstOrDefault();

            var standardApplicableRecord = standardApplicable?[RelationshipNames.EvidenceStandardApplicables];

            return standardApplicableRecord != null ?
                new StandardApplicable(standardApplicableRecord) : null;
        }

    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
