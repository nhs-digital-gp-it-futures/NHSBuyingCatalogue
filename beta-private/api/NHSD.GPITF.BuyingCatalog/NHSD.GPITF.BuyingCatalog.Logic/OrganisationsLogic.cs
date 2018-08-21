using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class OrganisationsLogic : LogicBase, IOrganisationsLogic
  {
    private readonly IOrganisationsDatastore _datastore;
    private readonly IOrganisationsValidator _validator;
    private readonly IOrganisationsFilter _filter;

    public OrganisationsLogic(
      IOrganisationsDatastore datastore,
      IHttpContextAccessor context,
      IOrganisationsValidator validator,
      IOrganisationsFilter filter
      ) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public Organisations ByODS(string odsCode)
    {
      return _filter.Filter(new[] { _datastore.ByODS(odsCode) }.AsQueryable()).SingleOrDefault();
    }

    public Organisations ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }.AsQueryable()).SingleOrDefault();
    }

    public IQueryable<Organisations> GetAll()
    {
      return _filter.Filter(_datastore.GetAll());
    }

    public Organisations Create(Organisations org)
    {
      _validator.ValidateAndThrow(org, ruleSet: nameof(IOrganisationsLogic.Create));
      return _datastore.Create(org);
    }

    public void Update(Organisations org)
    {
      _validator.ValidateAndThrow(org, ruleSet: nameof(IOrganisationsLogic.Update));
      _datastore.Update(org);
    }

    public void Delete(Organisations org)
    {
      _validator.ValidateAndThrow(org, ruleSet: nameof(IOrganisationsLogic.Delete));
      _datastore.Delete(org);
    }
  }
}
