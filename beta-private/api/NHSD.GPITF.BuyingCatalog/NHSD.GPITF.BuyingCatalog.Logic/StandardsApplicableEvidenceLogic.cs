using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableEvidenceLogic : LogicBase, IStandardsApplicableEvidenceLogic
  {
    private readonly IStandardsApplicableEvidenceDatastore _datastore;
    private readonly IContactsDatastore _contacts;

    public StandardsApplicableEvidenceLogic(
      IStandardsApplicableEvidenceDatastore datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
    }

    public IQueryable<StandardsApplicableEvidence> ByStandardsApplicable(string standardsApplicableId)
    {
      return _datastore.ByStandardsApplicable(standardsApplicableId);
    }

    public StandardsApplicableEvidence Create(StandardsApplicableEvidence evidence)
    {
      var email = Context.Email();
      evidence.CreatedById = _contacts.ByEmail(email).Id;
      return _datastore.Create(evidence);
    }
  }
}
