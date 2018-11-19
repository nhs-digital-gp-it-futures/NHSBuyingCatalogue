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

        public StandardApplicable ByEvidenceId(string id)
        {
            var evidence = ById(id);

            if (evidence == null)
                return null;

            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("StandardApplicableId") {FilterName = "cc_standardapplicableid", FilterValue = evidence.StandardApplicableId.ToString()},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new StandardApplicable().GetQueryString(null, filterAttributes), out Count);
            var standardApplicable = appJson?.FirstOrDefault();

            return new StandardApplicable(standardApplicable);
        }

        public StandardApplicable ByReviewId(string id)
        {
            StandardApplicable standardApplicable = null;

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
                    standardApplicable = ByEvidenceId(new Review(review).Evidence.ToString());
            }

            return standardApplicable;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
