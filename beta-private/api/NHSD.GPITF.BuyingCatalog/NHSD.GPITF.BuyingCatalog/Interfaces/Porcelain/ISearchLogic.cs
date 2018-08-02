using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain
{
#pragma warning disable CS1591
  public interface ISearchLogic
  {
    IQueryable<SolutionEx> SolutionExByKeyword(string keyword);
  }
#pragma warning restore CS1591
}
