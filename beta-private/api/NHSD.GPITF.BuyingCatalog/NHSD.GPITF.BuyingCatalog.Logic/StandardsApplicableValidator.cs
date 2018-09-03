using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableValidator : ClaimsValidatorBase<StandardsApplicable>, IStandardsApplicableValidator
  {
    public StandardsApplicableValidator(
      IHttpContextAccessor context,
      IStandardsApplicableDatastore claimDatastore) :
      base(context, claimDatastore)
    {
    }
  }
}
