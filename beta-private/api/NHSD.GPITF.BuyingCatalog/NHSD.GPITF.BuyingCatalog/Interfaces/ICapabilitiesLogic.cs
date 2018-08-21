using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ICapabilitiesLogic
  {
    IQueryable<Capabilities> ByFramework(string frameworkId);
    Capabilities ById(string id);
    IQueryable<Capabilities> ByIds(IEnumerable<string> ids);
    IQueryable<Capabilities> ByStandard(string standardId, bool isOptional);
    Capabilities Create(Capabilities capability);
    IQueryable<Capabilities> GetAll();
    void Update(Capabilities capability);
  }
#pragma warning restore CS1591
}
