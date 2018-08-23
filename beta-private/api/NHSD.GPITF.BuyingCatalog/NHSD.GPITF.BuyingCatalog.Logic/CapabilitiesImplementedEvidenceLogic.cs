using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedEvidenceLogic : LogicBase, ICapabilitiesImplementedEvidenceLogic
  {
    private readonly ICapabilitiesImplementedEvidenceDatastore _datastore;
    private readonly IContactsDatastore _contacts;

    public CapabilitiesImplementedEvidenceLogic(
      ICapabilitiesImplementedEvidenceDatastore datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
    }

    public IQueryable<CapabilitiesImplementedEvidence> ByCapabilitiesImplemented(string capabilitiesImplementedId)
    {
      return _datastore.ByCapabilitiesImplemented(capabilitiesImplementedId);
    }

    public CapabilitiesImplementedEvidence Create(CapabilitiesImplementedEvidence evidence)
    {
      var email = Context.Email();
      evidence.CreatedById = _contacts.ByEmail(email).Id;
      return _datastore.Create(evidence);
    }
  }
}
