using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class EvidenceLogicBase<T> : LogicBase where T : EvidenceBase
  {
    private readonly IEvidenceDatastore<T> _datastore;
    private readonly IContactsDatastore _contacts;

    public EvidenceLogicBase(
      IEvidenceDatastore<T> datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
    }

    public IQueryable<T> ByClaim(string claimId)
    {
      return _datastore.ByClaim(claimId);
    }

    public T Create(T evidence)
    {
      var email = Context.Email();
      evidence.CreatedById = _contacts.ByEmail(email).Id;
      return _datastore.Create(evidence);
    }
  }
}
