using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ICapabilitiesImplementedReviewsDatastore
  {
    IQueryable<CapabilitiesImplementedReviews> ByEvidence(string evidenceId);
  }
#pragma warning restore CS1591
}
