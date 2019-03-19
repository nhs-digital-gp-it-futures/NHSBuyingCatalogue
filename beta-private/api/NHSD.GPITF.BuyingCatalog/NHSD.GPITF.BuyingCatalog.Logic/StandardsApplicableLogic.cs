using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableLogic : ClaimsLogicBase<StandardsApplicable>, IStandardsApplicableLogic
  {
    public StandardsApplicableLogic(
      IStandardsApplicableDatastore datastore,
      IStandardsApplicableValidator validator,
      IStandardsApplicableFilter filter,
      IHttpContextAccessor context) :
      base(datastore, validator, filter, context)
    {
    }

    public override void Update(StandardsApplicable claim)
    {
      _validator.ValidateAndThrowEx(claim, ruleSet: nameof(IClaimsLogic<StandardsApplicable>.Update));

      if (claim.Status == StandardsApplicableStatus.Submitted)
      {
        claim.SubmittedOn = DateTime.UtcNow;
      }

      _datastore.Update(claim);
    }
  }
}
