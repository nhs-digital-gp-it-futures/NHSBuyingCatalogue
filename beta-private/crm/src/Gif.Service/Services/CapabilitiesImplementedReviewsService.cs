#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Attributes;
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Models;
using System.Collections.Generic;

namespace Gif.Service.Services
{
    public class CapabilitiesImplementedReviewsService : ServiceBase, ICapabilitiesImplementedReviewsDatastore
    {
        public CapabilitiesImplementedReviewsService(IRepository repository) : base(repository)
        {
        }

        public IEnumerable<Review> ByEvidence(string evidenceId)
        {
            var reviews = new List<Review>();

            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("Evidence") {FilterName = "_cc_evidence_value", FilterValue = evidenceId},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new Review().GetQueryString(null, filterAttributes, true, true), out Count);

            foreach (var review in appJson.Children())
            {
                reviews.Add(new Review(review));
            }

            var enumReviews = OrderLinkedReviews(reviews);

            Count = reviews.Count;

            return enumReviews;
        }

        public Review ById(string id)
        {
            throw new System.NotImplementedException();
        }

        public Review Create(Review review)
        {
            Repository.CreateEntity(review.EntityName, review.SerializeToODataPost());

            return review;
        }

        public void Delete(Review review)
        {
            throw new System.NotImplementedException();
        }

    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
