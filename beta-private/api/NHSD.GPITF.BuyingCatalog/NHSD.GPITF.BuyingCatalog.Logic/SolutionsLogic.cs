using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class SolutionsLogic : LogicBase, ISolutionsLogic
  {
    private readonly ISolutionsDatastore _datastore;
    private readonly IContactsDatastore _contacts;
    private readonly ISolutionsValidator _validator;
    private readonly ISolutionsFilter _filter;
    private readonly IEvidenceBlobStoreLogic _evidenceBlobStoreLogic;

    public SolutionsLogic(
      ISolutionsDatastore datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context,
      ISolutionsValidator validator,
      ISolutionsFilter filter,
      IEvidenceBlobStoreLogic evidenceBlobStoreLogic) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
      _validator = validator;
      _filter = filter;
      _evidenceBlobStoreLogic = evidenceBlobStoreLogic;
    }

    public IEnumerable<Solutions> ByFramework(string frameworkId)
    {
      return _filter.Filter(_datastore.ByFramework(frameworkId));
    }

    public Solutions ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }).SingleOrDefault();
    }

    public IEnumerable<Solutions> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }

    public Solutions Create(Solutions solution)
    {
      _validator.ValidateAndThrow(solution, ruleSet: nameof(ISolutionsLogic.Create));

      var email = Context.Email();
      solution.CreatedById = solution.ModifiedById = _contacts.ByEmail(email).Id;
      solution.CreatedOn = solution.ModifiedOn = DateTime.UtcNow;

      return _datastore.Create(solution);
    }

    public void Update(Solutions solution)
    {
      _validator.ValidateAndThrow(solution, ruleSet: nameof(ISolutionsLogic.Update));

      var email = Context.Email();
      solution.ModifiedById = _contacts.ByEmail(email).Id;
      solution.ModifiedOn = DateTime.UtcNow;

      _datastore.Update(solution);

      // create SharePoint folder structure
      if (solution.Status == SolutionStatus.Registered)
      {
        _evidenceBlobStoreLogic.PrepareForSolution(solution.Id);
      }
    }

    public void Delete(Solutions solution)
    {
      _validator.ValidateAndThrow(solution, ruleSet: nameof(ISolutionsLogic.Delete));

      _datastore.Delete(solution);
    }
  }
}
