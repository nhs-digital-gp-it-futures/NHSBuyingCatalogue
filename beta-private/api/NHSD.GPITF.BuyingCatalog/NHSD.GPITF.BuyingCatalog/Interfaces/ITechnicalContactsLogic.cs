using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ITechnicalContactsLogic
  {
    IQueryable<TechnicalContacts> BySolution(string solutionId);
    TechnicalContacts Create(TechnicalContacts techCont);
    void Update(TechnicalContacts techCont);
    void Delete(TechnicalContacts techCont);
  }
#pragma warning restore CS1591
}
