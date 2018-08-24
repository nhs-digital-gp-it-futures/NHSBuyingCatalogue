using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedLogic : LogicBase, ICapabilitiesImplementedLogic
  {
    private readonly ICapabilitiesImplementedDatastore _datastore;

    public CapabilitiesImplementedLogic(
      ICapabilitiesImplementedDatastore datastore, 
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public IQueryable<CapabilitiesImplemented> BySolution(string solutionId)
    {
      return _datastore.BySolution(solutionId);
    }

    public CapabilitiesImplemented Create(CapabilitiesImplemented claimedcapability)
    {
      return _datastore.Create(claimedcapability);
    }

    public void Delete(CapabilitiesImplemented claimedcapability)
    {
      _datastore.Delete(claimedcapability);
    }

    public void Update(CapabilitiesImplemented claimedcapability)
    {
      _datastore.Update(claimedcapability);
    }
  }
}
