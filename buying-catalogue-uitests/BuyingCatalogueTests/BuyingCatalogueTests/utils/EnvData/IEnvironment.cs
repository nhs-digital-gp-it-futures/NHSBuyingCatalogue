using System.Collections.Generic;

namespace BuyingCatalogueTests.utils.EnvData
{
    interface IEnvironment
    {
        string CatalogueUrl { get; set; }
        List<IUser> Users { get; set; }
        string CrmUrl { get; set; }
        IUser CrmUser { get; set; }
        string SharepointUrl { get; set; }
        IUser SharepointUser { get; set; }
    }
}
