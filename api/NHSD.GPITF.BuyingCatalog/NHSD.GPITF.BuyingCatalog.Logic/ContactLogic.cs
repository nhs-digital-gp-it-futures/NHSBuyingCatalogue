using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ContactLogic : LogicBase, IContactLogic
  {
    private readonly IContactDatastore _datastore;
    private readonly IContactValidator _validator;
    private readonly IContactFilter _filter;

    public ContactLogic(
      IContactDatastore datastore,
      IHttpContextAccessor context,
      IContactValidator validator,
      IContactFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public Contact ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }.AsQueryable()).SingleOrDefault();
    }

    public IQueryable<Contact> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }

    public Contact ByEmail(string email)
    {
      return _filter.Filter(new[] { _datastore.ByEmail(email) }.AsQueryable()).SingleOrDefault();
    }

    public Contact Create(Contact contact)
    {
      _validator.ValidateAndThrow(contact, ruleSet: nameof(IContactLogic.Create));
      return _datastore.Create(contact);
    }

    public void Update(Contact contact)
    {
      _validator.ValidateAndThrow(contact, ruleSet: nameof(IContactLogic.Update));
      _datastore.Update(contact);
    }

    public void Delete(Contact contact)
    {
      _validator.ValidateAndThrow(contact, ruleSet: nameof(IContactLogic.Delete));
      _datastore.Delete(contact);
    }
  }
}
