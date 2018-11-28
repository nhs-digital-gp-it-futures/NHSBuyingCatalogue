#pragma warning disable CS1591 
using Gif.Plugins.Contracts;

namespace Gif.Plugins.Business_Logic
{
    public class CascadeDeleteLogic : BusinessLogic, ICascadeDeleteLogic
    {

        #region Properties

        public ISolutionRepository CascadeDeleteRepository { get; set; }

        #endregion

        public CascadeDeleteLogic(ISolutionRepository cascadeDeleteRepository, string pluginName)
        {
            CascadeDeleteRepository = cascadeDeleteRepository;
            PluginName = pluginName;
        }
    }
}
#pragma warning restore CS1591