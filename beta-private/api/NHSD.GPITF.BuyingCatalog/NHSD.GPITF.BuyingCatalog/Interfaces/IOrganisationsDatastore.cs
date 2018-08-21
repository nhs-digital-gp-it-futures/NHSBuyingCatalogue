using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IOrganisationsDatastore
  {
    Organisations ByEmail(string email);
  }
#pragma warning restore CS1591
}
