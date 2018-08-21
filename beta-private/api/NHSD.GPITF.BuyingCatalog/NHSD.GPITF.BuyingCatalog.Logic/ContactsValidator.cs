using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ContactsValidator : ValidatorBase<Contacts>, IContactsValidator
  {
    public ContactsValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IContactsLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IContactsLogic.Update), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IContactsLogic.Delete), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
