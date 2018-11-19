#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Attributes;
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

        public IEnumerable<Evidence> ByClaim(string claimId)
        {
            var evidences = new List<Evidence>();

            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("ClaimId") {FilterName = "_cc_capabilityimplemented_value", FilterValue = claimId},
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

        public CapabilityImplemented ByEvidenceId(string id)
        {
            var evidence = ById(id);

            if (evidence == null)
                return null;

            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("CapabilityImplementedId") {FilterName = "cc_capabilityimplementedid", FilterValue = evidence.ClaimId.ToString()},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new CapabilityImplemented().GetQueryString(null, filterAttributes), out Count);
            var capabilityImplemented = appJson?.FirstOrDefault();

            return new CapabilityImplemented(capabilityImplemented);
        }

        public CapabilityImplemented ByReviewId(string id)
        {
            CapabilityImplemented capabilityImplemented = null;

            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("ReviewId") {FilterName = "cc_reviewid", FilterValue = id},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var reviewJson = Repository.RetrieveMultiple(new Review().GetQueryString(null, filterAttributes), out Count);
            var review = reviewJson?.FirstOrDefault();

            if (review != null)
            {
                var reviewObj = new Review(review);

                if (reviewObj.Evidence != null)
                    capabilityImplemented = ByEvidenceId(new Review(review).Evidence.ToString());
            }

            return capabilityImplemented;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
