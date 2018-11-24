#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Attributes;
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gif.Service.Services
{
    public class StandardsApplicableReviewsService : ServiceBase, IStandardsApplicableReviewsDatastore
    {
        public StandardsApplicableReviewsService(IRepository repository) : base(repository)
        {
        }

        public IEnumerable<IEnumerable<Review>> ByEvidence(string evidenceId)
        {
            var reviewList = new List<Review>();
            var reviewsListList = new List<List<Review>>();

            var filterReviewParent = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("EvidenceEntity") {FilterName = "_cc_evidence_value", FilterValue = evidenceId},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var jsonReviewParent = Repository.RetrieveMultiple(new Review().GetQueryString(null, filterReviewParent, true, true), out Count);

            foreach (var reviewChild in jsonReviewParent.Children())
            {
                var filterReviewChild = new List<CrmFilterAttribute>
                {
                    new CrmFilterAttribute("EvidenceEntity") {FilterName = "_cc_evidence_value", FilterValue = new Review(reviewChild).EvidenceId.ToString()},
                    new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
                };

                var jsonReviewChild = Repository.RetrieveMultiple(new Review().GetQueryString(null, filterReviewChild, true, true), out Count);
                foreach (var reviewChildChild in jsonReviewChild.Children())
                {
                    reviewList.Add(new Review(reviewChildChild));
                }

                var enumReviewList = Review.OrderLinkedReviews(reviewList);
                reviewsListList.Add(enumReviewList.ToList());
            }

            Count = reviewsListList.Count;

            return reviewsListList;
        }

        public Review ById(string id)
        {
            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("EvidenceId") {FilterName = "cc_reviewid", FilterValue = id},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new Review().GetQueryString(null, filterAttributes), out Count);
            var review = appJson?.FirstOrDefault();

            return new Review(review);
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
