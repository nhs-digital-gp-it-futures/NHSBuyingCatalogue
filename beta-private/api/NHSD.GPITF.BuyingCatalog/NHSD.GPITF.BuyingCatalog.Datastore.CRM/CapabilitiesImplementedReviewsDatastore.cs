using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesImplementedReviewsDatastore : ReviewsDatastoreBase<CapabilitiesImplementedReviews>, ICapabilitiesImplementedReviewsDatastore
  {
    private readonly GifInt.ICapabilitiesImplementedReviewsDatastore _crmDatastore;

    public CapabilitiesImplementedReviewsDatastore(
      GifInt.ICapabilitiesImplementedReviewsDatastore crmDatastore,
      ILogger<CrmDatastoreBase<CapabilitiesImplementedReviews>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    protected override IEnumerable<IEnumerable<CapabilitiesImplementedReviews>> ByEvidenceInternal(string evidenceId)
    {
      var retval = new List<IEnumerable<CapabilitiesImplementedReviews>>();
      var allVals = _crmDatastore
        .ByEvidence(evidenceId);
      foreach (var vals in allVals)
      {
        retval.Add(vals.Select(val => Converter.CapabilitiesImplementedReviewsFromCrm(val)));
      }

      return retval;
    }

    protected override CapabilitiesImplementedReviews ByIdInternal(string id)
    {
      var val = _crmDatastore
        .ById(id);

      return Converter.CapabilitiesImplementedReviewsFromCrm(val);
    }

    protected override CapabilitiesImplementedReviews CreateInternal(CapabilitiesImplementedReviews review)
    {
      var val = _crmDatastore
        .Create(Converter.FromApi(review));

      return Converter.CapabilitiesImplementedReviewsFromCrm(val);
    }

    protected override void DeleteInternal(CapabilitiesImplementedReviews review)
    {
      _crmDatastore
        .Delete(Converter.FromApi(review));
    }
  }
}
