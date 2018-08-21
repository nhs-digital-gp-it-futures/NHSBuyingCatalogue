using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ContactsValidator : ValidatorBase<Contacts>, IContactsValidator
  {
    public ContactsValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
