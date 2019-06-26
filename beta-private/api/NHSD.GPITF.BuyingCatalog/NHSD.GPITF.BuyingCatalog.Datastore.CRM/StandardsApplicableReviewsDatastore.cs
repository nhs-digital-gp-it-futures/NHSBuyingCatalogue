using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsApplicableReviewsDatastore : ReviewsDatastoreBase<StandardsApplicableReviews>, IStandardsApplicableReviewsDatastore
  {
    private readonly GifInt.IStandardsApplicableReviewsDatastore _crmDatastore;

    public StandardsApplicableReviewsDatastore(
      GifInt.IStandardsApplicableReviewsDatastore crmDatastore,
      ILogger<CrmDatastoreBase<StandardsApplicableReviews>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    protected override IEnumerable<IEnumerable<StandardsApplicableReviews>> ByEvidenceInternal(string evidenceId)
    {
      var retval = new List<IEnumerable<StandardsApplicableReviews>>();
      var allVals = _crmDatastore
        .ByEvidence(evidenceId);
      foreach (var vals in allVals)
      {
        retval.Add(vals.Select(val => Converter.StandardsApplicableReviewsFromCrm(val)));
      }

      return retval;
    }

    protected override StandardsApplicableReviews ByIdInternal(string id)
    {
      var val = _crmDatastore
        .ById(id);

      return Converter.StandardsApplicableReviewsFromCrm(val);
    }

    protected override StandardsApplicableReviews CreateInternal(StandardsApplicableReviews review)
    {
      var val = _crmDatastore
        .Create(Converter.FromApi(review));

      return Converter.StandardsApplicableReviewsFromCrm(val);
    }

    protected override void DeleteInternal(StandardsApplicableReviews review)
    {
      _crmDatastore
        .Delete(Converter.FromApi(review));
    }
  }
}
