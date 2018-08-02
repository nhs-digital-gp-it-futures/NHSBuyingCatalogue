using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedCapabilityLogic : LogicBase, IClaimedCapabilityLogic
  {
    private readonly IClaimedCapabilityDatastore _datastore;
    private readonly IClaimedCapabilityValidator _validator;
    private readonly IClaimedCapabilityFilter _filter;

    public ClaimedCapabilityLogic(
      IClaimedCapabilityDatastore datastore, 
      IHttpContextAccessor context,
      IClaimedCapabilityValidator validator,
      IClaimedCapabilityFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<ClaimedCapability> BySolution(string solutionId)
    {
      var claimedCaps = _datastore.BySolution(solutionId);
      claimedCaps.ToList().ForEach(cc => _validator.ValidateAndThrow(cc));
      return _filter.Filter(claimedCaps);
    }

    public ClaimedCapability Create(ClaimedCapability claimedcapability)
    {
      _validator.ValidateAndThrow(claimedcapability);
      return _filter.Filter(new[] { _datastore.Create(claimedcapability) }.AsQueryable()).SingleOrDefault();
    }

    public void Delete(ClaimedCapability claimedcapability)
    {
      _validator.ValidateAndThrow(claimedcapability);
      _datastore.Delete(claimedcapability);
    }

    public void Update(ClaimedCapability claimedcapability)
    {
      _validator.ValidateAndThrow(claimedcapability);
      _datastore.Update(claimedcapability);
    }
  }
}
