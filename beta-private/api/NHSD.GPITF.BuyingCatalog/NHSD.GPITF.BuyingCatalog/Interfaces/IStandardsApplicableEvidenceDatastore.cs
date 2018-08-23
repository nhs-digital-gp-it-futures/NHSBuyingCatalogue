using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IStandardsApplicableEvidenceDatastore
  {
    IQueryable<StandardsApplicableEvidence> ByStandardsApplicable(string standardsApplicableId);
    StandardsApplicableEvidence Create(StandardsApplicableEvidence evidence);
  }
#pragma warning restore CS1591
}
