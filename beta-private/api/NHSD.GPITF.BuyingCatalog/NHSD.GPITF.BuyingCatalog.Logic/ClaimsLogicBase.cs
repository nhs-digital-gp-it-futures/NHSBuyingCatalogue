using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ClaimsLogicBase<T> : LogicBase where T : ClaimsBase
  {
    private readonly IClaimsDatastore<T> _datastore;

    public ClaimsLogicBase(
      IClaimsDatastore<T> datastore,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public T ById(string id)
    {
      return _datastore.ById(id);
    }

    public IEnumerable<T> BySolution(string solutionId)
    {
      return _datastore.BySolution(solutionId);
    }

    public T Create(T claim)
    {
      return _datastore.Create(claim);
    }

    public void Update(T claim)
    {
      _datastore.Update(claim);
    }

    public void Delete(T claim)
    {
      _datastore.Delete(claim);
    }
  }
}
