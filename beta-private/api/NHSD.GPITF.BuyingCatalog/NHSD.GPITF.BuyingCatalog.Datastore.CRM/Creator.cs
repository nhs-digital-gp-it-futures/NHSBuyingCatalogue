using NHSD.GPITF.BuyingCatalog.Models;
using GifModels = Gif.Service.Models;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  internal static class Creator
  {
    internal static Frameworks FromCrm(GifModels.Framework crm)
    {
      return new Frameworks
      {
        Id = crm.Id.ToString(),
        PreviousId = crm.PreviousId?.ToString(),
        Name = crm.Name,
        Description = crm.Description
      };
    }
  }
}
