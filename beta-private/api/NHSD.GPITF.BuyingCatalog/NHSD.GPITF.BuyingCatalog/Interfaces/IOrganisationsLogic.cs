using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IOrganisationsLogic
  {
    Organisations ByODS(string odsCode);
  }
#pragma warning restore CS1591
}
