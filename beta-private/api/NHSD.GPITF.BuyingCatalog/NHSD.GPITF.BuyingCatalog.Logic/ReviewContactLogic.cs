using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ReviewContactLogic : LogicBase, IReviewContactLogic
  {
    private readonly IReviewContactDatastore _datastore;

    public ReviewContactLogic(IReviewContactDatastore datastore, IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public IQueryable<ReviewContact> ByAssessmentMessage(string assMessId)
    {
      return _datastore.ByAssessmentMessage(assMessId);
    }

    public ReviewContact Create(ReviewContact assMessCont)
    {
      return _datastore.Create(assMessCont);
    }

    public void Delete(ReviewContact assMess)
    {
      throw new NotImplementedException("Cannot change history");
    }

    public void Update(ReviewContact assMess)
    {
      throw new NotImplementedException("Cannot change history");
    }
  }
}
