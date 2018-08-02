using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class AssessmentMessageContactLogic : LogicBase, IAssessmentMessageContactLogic
  {
    private readonly IAssessmentMessageContactDatastore _datastore;

    public AssessmentMessageContactLogic(IAssessmentMessageContactDatastore datastore, IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public IQueryable<AssessmentMessageContact> ByAssessmentMessage(string assMessId)
    {
      return _datastore.ByAssessmentMessage(assMessId);
    }

    public AssessmentMessageContact Create(AssessmentMessageContact assMessCont)
    {
      return _datastore.Create(assMessCont);
    }

    public void Delete(AssessmentMessageContact assMess)
    {
      throw new NotImplementedException("Cannot change history");
    }

    public void Update(AssessmentMessageContact assMess)
    {
      throw new NotImplementedException("Cannot change history");
    }
  }
}
