using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ContactValidator : ValidatorBase<Contact>, IContactValidator
  {
    public ContactValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IContactLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IContactLogic.Update), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IContactLogic.Delete), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
