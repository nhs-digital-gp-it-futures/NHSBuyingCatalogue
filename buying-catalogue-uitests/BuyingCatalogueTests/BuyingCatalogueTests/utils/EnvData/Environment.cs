using System.Collections.Generic;

namespace BuyingCatalogueTests.utils.EnvData
{
    internal sealed class Environment : IEnvironment
    {
        public string CatalogueUrl { get; set; }
        public List<IUser> Users { get; set; }
        public string CrmUrl { get; set; }
        public IUser CrmUser { get; set; }
        public string SharepointUrl { get; set; }
        public IUser SharepointUser { get; set; }
    }
}