using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ITechnicalContactLogic
  {
    IQueryable<TechnicalContact> BySolution(string solutionId);
    TechnicalContact Create(TechnicalContact techCont);
    void Update(TechnicalContact techCont);
    void Delete(TechnicalContact techCont);
  }
#pragma warning restore CS1591
}
