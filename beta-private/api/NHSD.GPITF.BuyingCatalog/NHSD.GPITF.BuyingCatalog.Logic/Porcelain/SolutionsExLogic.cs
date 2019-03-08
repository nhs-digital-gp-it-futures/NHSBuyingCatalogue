using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SolutionsExLogic : LogicBase, ISolutionsExLogic
  {
    private readonly ISolutionsExDatastore _datastore;
    private readonly ISolutionsExValidator _validator;
    private readonly ISolutionsExFilter _filter;
    private readonly IContactsDatastore _contacts;
    private readonly IEvidenceBlobStoreLogic _evidenceBlobStoreLogic;

    public SolutionsExLogic(
      ISolutionsExDatastore datastore,
      IHttpContextAccessor context,
      ISolutionsExValidator validator,
      ISolutionsExFilter filter,
      IContactsDatastore contacts,
      IEvidenceBlobStoreLogic evidenceBlobStoreLogic) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
      _contacts = contacts;
      _evidenceBlobStoreLogic = evidenceBlobStoreLogic;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return _filter.Filter(new[] { _datastore.BySolution(solutionId) }).SingleOrDefault();
    }

    public void Update(SolutionEx solnEx)
    {
      _validator.ValidateAndThrowEx(solnEx, ruleSet: nameof(ISolutionsExLogic.Update));

      var email = Context.Email();
      solnEx.Solution.ModifiedById = _contacts.ByEmail(email).Id;
      solnEx.Solution.ModifiedOn = DateTime.UtcNow;


      solnEx.ClaimedCapabilityReview.ForEach(review =>
      {
         review.OriginalDate = (review.OriginalDate == default(DateTime)) ? DateTime.UtcNow : review.OriginalDate;
      });
      solnEx.ClaimedStandardReview.ForEach(review =>
      {
         review.OriginalDate = (review.OriginalDate == default(DateTime)) ? DateTime.UtcNow : review.OriginalDate;
      });

      solnEx.ClaimedCapabilityEvidence.ForEach(evidence =>
      {
         evidence.OriginalDate = (evidence.OriginalDate == default(DateTime)) ? DateTime.UtcNow : evidence.OriginalDate;
      });
      solnEx.ClaimedStandardEvidence.ForEach(evidence =>
      {
         evidence.OriginalDate = (evidence.OriginalDate == default(DateTime)) ? DateTime.UtcNow : evidence.OriginalDate;
      });


      _datastore.Update(solnEx);

      // create SharePoint folder structure
      if (solnEx.Solution.Status == SolutionStatus.Registered)
      {
        _evidenceBlobStoreLogic.PrepareForSolution(solnEx.Solution.Id);
      }
    }

    public IEnumerable<SolutionEx> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }
  }
}
