using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class LinkManagerLogic : LogicBase, ILinkManagerLogic
  {
    private readonly ILinkManagerDatastore _datastore;
    private readonly ILinkManagerValidator _validator;

    public LinkManagerLogic(
      ILinkManagerDatastore datastore,
      IHttpContextAccessor context,
      ILinkManagerValidator validator) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
    }

    public void CapabilityFrameworkCreate(string frameworkId, string capabilityId)
    {
      _validator.ValidateAndThrow(Context);
      _datastore.CapabilityFrameworkCreate(frameworkId, capabilityId);
    }

    public void CapabilityStandardCreate(string capabilityId, string standardId, bool isOptional)
    {
      _validator.ValidateAndThrow(Context);
      _datastore.CapabilityStandardCreate(capabilityId, standardId, isOptional);
    }

    public void FrameworkSolutionCreate(string frameworkId, string solutionId)
    {
      _validator.ValidateAndThrow(Context);
      _datastore.FrameworkSolutionCreate(frameworkId, solutionId);
    }

    public void FrameworkStandardCreate(string frameworkId, string standardId)
    {
      _validator.ValidateAndThrow(Context);
      _datastore.FrameworkStandardCreate(frameworkId, standardId);
    }
  }
}
