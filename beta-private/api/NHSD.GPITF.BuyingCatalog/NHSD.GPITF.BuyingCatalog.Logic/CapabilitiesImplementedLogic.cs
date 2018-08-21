using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedLogic : LogicBase, ICapabilitiesImplementedLogic
  {
    private readonly ICapabilitiesImplementedDatastore _datastore;
    private readonly ICapabilitiesImplementedValidator _validator;
    private readonly ICapabilitiesImplementedFilter _filter;

    public CapabilitiesImplementedLogic(
      ICapabilitiesImplementedDatastore datastore, 
      IHttpContextAccessor context,
      ICapabilitiesImplementedValidator validator,
      ICapabilitiesImplementedFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<CapabilitiesImplemented> BySolution(string solutionId)
    {
      var claimedCaps = _datastore.BySolution(solutionId);
      claimedCaps.ToList().ForEach(cc => _validator.ValidateAndThrow(cc));
      return _filter.Filter(claimedCaps);
    }

    public CapabilitiesImplemented Create(CapabilitiesImplemented claimedcapability)
    {
      _validator.ValidateAndThrow(claimedcapability);
      return _filter.Filter(new[] { _datastore.Create(claimedcapability) }.AsQueryable()).SingleOrDefault();
    }

    public void Delete(CapabilitiesImplemented claimedcapability)
    {
      _validator.ValidateAndThrow(claimedcapability);
      _datastore.Delete(claimedcapability);
    }

    public void Update(CapabilitiesImplemented claimedcapability)
    {
      _validator.ValidateAndThrow(claimedcapability);
      _datastore.Update(claimedcapability);
    }
  }
}
