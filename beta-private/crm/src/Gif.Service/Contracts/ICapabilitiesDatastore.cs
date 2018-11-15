using Gif.Service.Models;
using System.Collections.Generic;

namespace Gif.Service.Contracts
{
#pragma warning disable CS1591
    public interface ICapabilityDatastore
    {
        IEnumerable<Capability> ByFramework(string frameworkId);
        Capability ById(string id);
        IEnumerable<Capability> ByIds(IEnumerable<string> ids);
        IEnumerable<Capability> ByStandard(string standardId, bool isOptional);
        IEnumerable<Capability> GetAll();
    }
#pragma warning restore CS1591
}
