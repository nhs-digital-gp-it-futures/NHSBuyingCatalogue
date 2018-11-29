using Microsoft.Xrm.Sdk;

#pragma warning disable CS1591 
namespace Gif.Plugins.Contracts
{
    public interface ICascadeDeleteLogic
    {
        void OnSolutionDelete(EntityReference target);
    }
}
#pragma warning restore CS1591