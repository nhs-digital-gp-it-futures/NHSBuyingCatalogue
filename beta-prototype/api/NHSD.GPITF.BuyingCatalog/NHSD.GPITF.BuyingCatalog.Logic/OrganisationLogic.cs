using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class OrganisationLogic : LogicBase, IOrganisationLogic
  {
    private readonly IOrganisationDatastore _datastore;
    private readonly IOrganisationValidator _validator;
    private readonly IOrganisationFilter _filter;

    public OrganisationLogic(
      IOrganisationDatastore datastore,
      IHttpContextAccessor context,
      IOrganisationValidator validator,
      IOrganisationFilter filter
      ) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public Organisation ByODS(string odsCode)
    {
      return _filter.Filter(new[] { _datastore.ByODS(odsCode) }.AsQueryable()).SingleOrDefault();
    }

    public Organisation ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }.AsQueryable()).SingleOrDefault();
    }

    public IQueryable<Organisation> GetAll()
    {
      return _filter.Filter(_datastore.GetAll());
    }

    public Organisation Create(Organisation org)
    {
      _validator.ValidateAndThrow(org, ruleSet: nameof(IOrganisationLogic.Create));
      return _datastore.Create(org);
    }

    public void Update(Organisation org)
    {
      _validator.ValidateAndThrow(org, ruleSet: nameof(IOrganisationLogic.Update));
      _datastore.Update(org);
    }

    public void Delete(Organisation org)
    {
      _validator.ValidateAndThrow(org, ruleSet: nameof(IOrganisationLogic.Delete));
      _datastore.Delete(org);
    }
  }
}
