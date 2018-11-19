using Gif.Service.Models;
//using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace Gif.Service.Contracts
{
#pragma warning disable CS1591
    public interface ICapabilitiesImplementedEvidenceDatastore : IEvidenceDatastore<Evidence>
    {
        CapabilityImplemented ByEvidenceId(string id);
        CapabilityImplemented ByReviewId(string id);
    }
#pragma warning restore CS1591
}
