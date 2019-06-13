using System.Collections.Generic;

namespace BuyingCatalogueTests.utils.EnvData
{
    interface IEnvironment
    {
        string CatalogueUrl { get; set; }
        IList<IUser> Users { get; set; }
    }
}
