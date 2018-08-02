using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedStandardLogic : LogicBase, IClaimedStandardLogic
  {
    private readonly IClaimedStandardDatastore _datastore;
    private readonly IClaimedStandardValidator _validator;
    private readonly IClaimedStandardFilter _filter;

    public ClaimedStandardLogic(
      IClaimedStandardDatastore datastore,
      IHttpContextAccessor context,
      IClaimedStandardValidator validator,
      IClaimedStandardFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<ClaimedStandard> BySolution(string solutionId)
    {
      var claimedStds = _datastore.BySolution(solutionId);
      claimedStds.ToList().ForEach(cs => _validator.ValidateAndThrow(cs));
      return _filter.Filter(claimedStds);
    }

    public ClaimedStandard Create(ClaimedStandard claimedstandard)
    {
      _validator.ValidateAndThrow(claimedstandard);
      return _filter.Filter(new[] { _datastore.Create(claimedstandard) }.AsQueryable()).SingleOrDefault();
    }

    public void Update(ClaimedStandard claimedstandard)
    {
      _validator.ValidateAndThrow(claimedstandard);
      _datastore.Update(claimedstandard);
    }

    public void Delete(ClaimedStandard claimedstandard)
    {
      _validator.ValidateAndThrow(claimedstandard);
      _datastore.Delete(claimedstandard);
    }
  }
}
