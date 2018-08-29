using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SearchLogic : LogicBase, ISearchLogic
  {
    private readonly ISearchDatastore _datastore;
    private readonly ISolutionsFilter _solutionFilter;

    public SearchLogic(
      IHttpContextAccessor context,
      ISearchDatastore datastore,
      ISolutionsFilter solutionFilter) :
      base(context)
    {
      _datastore = datastore;
      _solutionFilter = solutionFilter;
    }

    public IEnumerable<SolutionEx> SolutionExByKeyword(string keyword)
    {
      var solnExs = _datastore.SolutionExByKeyword(keyword);
      return solnExs.Where(soln => _solutionFilter.Filter(new[] { soln.Solution }).Any());
    }
  }
}
