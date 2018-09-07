using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedEvidenceLogic : EvidenceLogicBase<CapabilitiesImplementedEvidence>, ICapabilitiesImplementedEvidenceLogic
  {
    public CapabilitiesImplementedEvidenceLogic(
      ICapabilitiesImplementedEvidenceDatastore datastore,
      IContactsDatastore contacts,
      ICapabilitiesImplementedEvidenceValidator validator,
      ICapabilitiesImplementedEvidenceFilter filter,
      IHttpContextAccessor context) :
      base(datastore, contacts, validator, filter, context)
    {
    }
  }
}
