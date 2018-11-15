using Gif.Service.Models;
using System.Collections.Generic;

namespace Gif.Service.Contracts
{
#pragma warning disable CS1591
    public interface IStandardsApplicableReviewsDatastore
    {
        IEnumerable<Review> ByEvidence(string evidenceId);
        Review ById(string id);
        Review Create(Review review);
        void Delete(Review review);
    }
#pragma warning restore CS1591
}
