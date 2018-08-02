using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IStandardLogic
  {
    IQueryable<Standard> ByCapability(string capabilityId, bool isOptional);
    IQueryable<Standard> ByFramework(string frameworkId);
    Standard ById(string id);
    IQueryable<Standard> ByIds(IEnumerable<string> ids);
    Standard Create(Standard standard);
    IQueryable<Standard> GetAll();
    void Update(Standard standard);
  }
#pragma warning restore CS1591
}
