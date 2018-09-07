using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class EvidenceLogicBase<T> : LogicBase where T : EvidenceBase
  {
    private readonly IEvidenceDatastore<T> _datastore;
    private readonly IContactsDatastore _contacts;
    private readonly IEvidenceValidator<T> _validator;
    private readonly IEvidenceFilter<IEnumerable<T>> _filter;

    public EvidenceLogicBase(
      IEvidenceDatastore<T> datastore,
      IContactsDatastore contacts,
      IEvidenceValidator<T> validator,
      IEvidenceFilter<IEnumerable<T>> filter,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
      _validator = validator;
      _filter = filter;
    }

    public IEnumerable<IEnumerable<T>> ByClaim(string claimId)
    {
      return _filter.Filter(_datastore.ByClaim(claimId));
    }

    public T Create(T evidence)
    {
      _validator.ValidateAndThrow(evidence);
      _validator.ValidateAndThrow(evidence, ruleSet: nameof(IEvidenceLogic<T>.Create));

      var email = Context.Email();
      evidence.CreatedById = _contacts.ByEmail(email).Id;
      return _datastore.Create(evidence);
    }
  }
}
