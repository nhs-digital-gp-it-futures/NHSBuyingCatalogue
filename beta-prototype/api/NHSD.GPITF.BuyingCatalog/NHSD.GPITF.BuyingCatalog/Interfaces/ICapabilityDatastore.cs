using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ICapabilityDatastore
  {
    IQueryable<Capability> ByFramework(string frameworkId);
    Capability ById(string id);
    IQueryable<Capability> ByIds(IEnumerable<string> ids);
    IQueryable<Capability> ByStandard(string standardId, bool isOptional);
    Capability Create(Capability capability);
    IQueryable<Capability> GetAll();
    void Update(Capability capability);
  }
#pragma warning restore CS1591
}
