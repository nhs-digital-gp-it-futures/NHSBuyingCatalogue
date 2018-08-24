using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableLogic : LogicBase, IStandardsApplicableLogic
  {
    private readonly IStandardsApplicableDatastore _datastore;

    public StandardsApplicableLogic(
      IStandardsApplicableDatastore datastore,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public IQueryable<StandardsApplicable> BySolution(string solutionId)
    {
      return _datastore.BySolution(solutionId);
    }

    public StandardsApplicable Create(StandardsApplicable claimedstandard)
    {
      return _datastore.Create(claimedstandard);
    }

    public void Update(StandardsApplicable claimedstandard)
    {
      _datastore.Update(claimedstandard);
    }

    public void Delete(StandardsApplicable claimedstandard)
    {
      _datastore.Delete(claimedstandard);
    }
  }
}
