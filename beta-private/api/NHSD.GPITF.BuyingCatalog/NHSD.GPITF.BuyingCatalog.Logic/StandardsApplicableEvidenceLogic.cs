using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableEvidenceLogic : EvidenceLogicBase<StandardsApplicableEvidence>, IStandardsApplicableEvidenceLogic
  {
    public StandardsApplicableEvidenceLogic(
      IStandardsApplicableEvidenceDatastore datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context) :
      base(datastore, contacts, context)
    {
    }
  }
}
