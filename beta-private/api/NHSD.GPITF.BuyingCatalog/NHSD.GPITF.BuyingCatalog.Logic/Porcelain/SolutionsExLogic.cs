using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SolutionsExLogic : LogicBase, ISolutionsExLogic
  {
    private readonly ISolutionsExDatastore _datastore;

    public SolutionsExLogic(ISolutionsExDatastore datastore, IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return _datastore.BySolution(solutionId);
    }

    public void Update(SolutionEx solnEx)
    {
      _datastore.Update(solnEx);
    }
  }
}
