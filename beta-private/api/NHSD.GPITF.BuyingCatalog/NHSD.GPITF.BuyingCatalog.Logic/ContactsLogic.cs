using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ContactsLogic : LogicBase, IContactsLogic
  {
    private readonly IContactsDatastore _datastore;
    private readonly IContactsValidator _validator;
    private readonly IContactsFilter _filter;

    public ContactsLogic(
      IContactsDatastore datastore,
      IHttpContextAccessor context,
      IContactsValidator validator,
      IContactsFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public Contacts ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }.AsQueryable()).SingleOrDefault();
    }

    public IQueryable<Contacts> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }

    public Contacts ByEmail(string email)
    {
      return _filter.Filter(new[] { _datastore.ByEmail(email) }.AsQueryable()).SingleOrDefault();
    }

    public Contacts Create(Contacts contact)
    {
      _validator.ValidateAndThrow(contact, ruleSet: nameof(IContactsLogic.Create));
      return _datastore.Create(contact);
    }

    public void Update(Contacts contact)
    {
      _validator.ValidateAndThrow(contact, ruleSet: nameof(IContactsLogic.Update));
      _datastore.Update(contact);
    }

    public void Delete(Contacts contact)
    {
      _validator.ValidateAndThrow(contact, ruleSet: nameof(IContactsLogic.Delete));
      _datastore.Delete(contact);
    }
  }
}
