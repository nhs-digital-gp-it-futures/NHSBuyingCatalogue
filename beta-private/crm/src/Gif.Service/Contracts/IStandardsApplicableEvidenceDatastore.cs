using Gif.Service.Models;

namespace Gif.Service.Contracts
{
#pragma warning disable CS1591
    public interface IStandardsApplicableEvidenceDatastore : IEvidenceDatastore<StandardEvidence>
    {
        StandardApplicable ByEvidenceId(string id);

    }
#pragma warning restore CS1591
}
