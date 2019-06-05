using System.Collections.Generic;

namespace BuyingCatalogueTests.utils.EnvData
{
    internal class Environment : IEnvironment
    {
        public string CatalogueUrl { get; set; }
        public List<User> Users { get; set; }
        public string CrmUrl { get; set; }
        public User CrmUser { get; set; }
        public string SharepointUrl { get; set; }
        public User SharepointUser { get; set; }
    }
}