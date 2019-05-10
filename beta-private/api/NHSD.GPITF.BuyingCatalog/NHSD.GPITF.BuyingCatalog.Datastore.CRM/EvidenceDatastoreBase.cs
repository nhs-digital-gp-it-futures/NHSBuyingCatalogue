using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class EvidenceDatastoreBase<T> : CrmDatastoreBase<T>, IEvidenceDatastore<EvidenceBase> where T : EvidenceBase
  {
    protected EvidenceDatastoreBase(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
    }

    protected abstract IEnumerable<IEnumerable<T>> ByClaimInternal(string claimId);
    public IEnumerable<IEnumerable<T>> ByClaim(string claimId)
    {
      return GetInternal(() =>
      {
        return ByClaimInternal(claimId);
      });
    }

    protected abstract T ByIdInternal(string id);
    public T ById(string id)
    {
      return GetInternal(() =>
      {
        return ByIdInternal(id);
      });
    }

    protected abstract T CreateInternal(T evidence);
    public T Create(T evidence)
    {
      return GetInternal(() =>
      {
        evidence.Id = UpdateId(evidence.Id);

        return CreateInternal(evidence);
      });
    }

    IEnumerable<IEnumerable<EvidenceBase>> IEvidenceDatastore<EvidenceBase>.ByClaim(string claimId)
    {
      return ByClaim(claimId);
    }

    EvidenceBase IEvidenceDatastore<EvidenceBase>.ById(string id)
    {
      return ById(id);
    }

    public EvidenceBase Create(EvidenceBase evidence)
    {
      return Create((T)evidence);
    }
  }
}
