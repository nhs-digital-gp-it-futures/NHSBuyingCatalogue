using Gif.Service.Models;
using System.Collections.Generic;
//using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace Gif.Service.Contracts
{
#pragma warning disable CS1591
    public interface ICapabilitiesImplementedReviewsDatastore
    {
        IEnumerable<Review> ByEvidence(string evidenceId);
        Review ById(string id);
        Review Create(Review review);
        void Delete(Review review);
    }
#pragma warning restore CS1591
}
