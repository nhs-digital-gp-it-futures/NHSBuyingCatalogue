using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class ClaimsDatastoreBase<T> : CrmDatastoreBase<T>, IClaimsDatastore<ClaimsBase> where T : ClaimsBase
  {
    public ClaimsDatastoreBase(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
    }

    protected abstract T ByIdInternal(string id);
    public T ById(string id)
    {
      return GetInternal(() =>
      {
        return ByIdInternal(id);
      });
    }

    protected abstract IEnumerable<T> BySolutionInternal(string solutionId);
    public IEnumerable<T> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return BySolutionInternal(solutionId);
      });
    }

    protected abstract T CreateInternal(T claim);
    public T Create(T claim)
    {
      return GetInternal(() =>
      {
        claim.Id = UpdateId(claim.Id);

        return CreateInternal(claim);
      });
    }

    protected abstract void DeleteInternal(T claim);
    public void Delete(T claim)
    {
      GetInternal(() =>
      {
        DeleteInternal(claim);

        return 0;
      });
    }

    protected abstract void UpdateInternal(T claim);
    public void Update(T claim)
    {
      GetInternal(() =>
      {
        UpdateInternal(claim);

        return 0;
      });
    }

    ClaimsBase IClaimsDatastore<ClaimsBase>.ById(string id)
    {
      return ById(id);
    }

    IEnumerable<ClaimsBase> IClaimsDatastore<ClaimsBase>.BySolution(string solutionId)
    {
      return BySolution(solutionId);
    }

    public ClaimsBase Create(ClaimsBase claim)
    {
      return Create((T)claim);
    }

    public void Update(ClaimsBase claim)
    {
      Update((T)claim);
    }

    public void Delete(ClaimsBase claim)
    {
      Delete((T)claim);
    }
  }
}
