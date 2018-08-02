using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedCapabilityStandardLogic : LogicBase, IClaimedCapabilityStandardLogic
  {
    private readonly IClaimedCapabilityStandardDatastore _datastore;
    private readonly IClaimedCapabilityStandardValidator _validator;
    private readonly IClaimedCapabilityStandardFilter _filter;

    public ClaimedCapabilityStandardLogic(
      IClaimedCapabilityStandardDatastore datastore,
      IHttpContextAccessor context,
      IClaimedCapabilityStandardValidator validator,
      IClaimedCapabilityStandardFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<ClaimedCapabilityStandard> ByClaimedCapability(string claimedCapabilityId)
    {
      var claimedCapStds = _datastore.ByClaimedCapability(claimedCapabilityId);
      claimedCapStds.ToList().ForEach(ccs => _validator.ValidateAndThrow(ccs));
      return _filter.Filter(claimedCapStds);
    }

    public IQueryable<ClaimedCapabilityStandard> ByStandard(string standardId)
    {
      var claimedCapStds = _datastore.ByStandard(standardId);
      claimedCapStds.ToList().ForEach(ccs => _validator.ValidateAndThrow(ccs));
      return _filter.Filter(claimedCapStds);
    }

    public ClaimedCapabilityStandard Create(ClaimedCapabilityStandard claimedCapStd)
    {
      _validator.ValidateAndThrow(claimedCapStd);
      return _filter.Filter(new[] { _datastore.Create(claimedCapStd) }.AsQueryable()).SingleOrDefault();
    }

    public void Delete(ClaimedCapabilityStandard claimedCapStd)
    {
      _validator.ValidateAndThrow(claimedCapStd);
      _datastore.Delete(claimedCapStd);
    }

    public void Update(ClaimedCapabilityStandard claimedCapStd)
    {
      _validator.ValidateAndThrow(claimedCapStd);
      _datastore.Update(claimedCapStd);
    }
  }
}
