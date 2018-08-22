using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IStandardsDatastore
  {
    IQueryable<Standards> ByCapability(string capabilityId, bool isOptional);
    IQueryable<Standards> ByFramework(string frameworkId);
    Standards ById(string id);
    IQueryable<Standards> ByIds(IEnumerable<string> ids);
    IQueryable<Standards> GetAll();
  }
#pragma warning restore CS1591
}
