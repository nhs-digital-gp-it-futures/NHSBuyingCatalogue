using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain
{
#pragma warning disable CS1591
  public interface ISearchLogic
  {
    IEnumerable<SolutionEx> SolutionExByKeyword(string keyword);
  }
#pragma warning restore CS1591
}
