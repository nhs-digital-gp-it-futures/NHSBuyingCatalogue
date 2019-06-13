using System.Collections.Generic;

namespace BuyingCatalogueTests.utils.EnvData
{
    internal sealed class Environment : IEnvironment
    {
        public string CatalogueUrl { get; set; }
        public IList<IUser> Users { get; set; }
    }
}