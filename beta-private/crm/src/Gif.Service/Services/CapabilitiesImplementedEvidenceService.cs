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
    public class CapabilitiesImplementedEvidenceService : ServiceBase, ICapabilitiesImplementedEvidenceDatastore
    {
        public CapabilitiesImplementedEvidenceService(IRepository repository) : base(repository)
        {
        }

        public IEnumerable<CapabilityEvidence> ByClaim(string claimId)
        {
            var evidences = new List<CapabilityEvidence>();

            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("ClaimId") {FilterName = "_cc_capabilityimplemented_value", FilterValue = claimId},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new CapabilityEvidence().GetQueryString(null, filterAttributes, true, true), out Count);

            foreach (var evidence in appJson.Children())
            {
                evidences.Add(new CapabilityEvidence(evidence));
            }

            var enumEvidences = CapabilityEvidence.OrderLinkedEvidences(evidences);

            Count = evidences.Count;

            return enumEvidences;
        }

        IEnumerable<IEnumerable<CapabilityEvidence>> IEvidenceDatastore<CapabilityEvidence>.ByClaim(string claimId)
        {
            throw new System.NotImplementedException();
        }

        public CapabilityEvidence ById(string id)
        {
            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("EvidenceId") {FilterName = "cc_evidenceid", FilterValue = id},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new CapabilityEvidence().GetQueryString(null, filterAttributes), out Count);
            var evidence = appJson?.FirstOrDefault();

            return new CapabilityEvidence(evidence);
        }

        public CapabilityEvidence Create(CapabilityEvidence evidenceEntity)
        {
            Repository.CreateEntity(evidenceEntity.EntityName, evidenceEntity.SerializeToODataPost());

            return evidenceEntity;
        }

        public CapabilityImplemented ByEvidenceId(string id)
        {
            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("EvidenceId") {FilterName = "cc_evidenceid", FilterValue = id},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new CapabilityImplementedEvidence().GetQueryString(null, filterAttributes, true), out Count);
            var capabilityImplemented = appJson?.Children().FirstOrDefault();

            var capabilityImplementedRecord = capabilityImplemented?[RelationshipNames.EvidenceCapabilityImplemented];

            return capabilityImplementedRecord != null ?
                new CapabilityImplemented(capabilityImplementedRecord) : null;
        }

    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
