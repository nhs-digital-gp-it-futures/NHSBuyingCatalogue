using System.Collections.Generic;

namespace BuyingCatalogueTests.utils.EnvData
{
    interface IEnvironment
    {
        string CatalogueUrl { get; set; }
        List<User> Users { get; set; }
        string CrmUrl { get; set; }
        User CrmUser { get; set; }
        string SharepointUrl { get; set; }
        User SharepointUser { get; set; }
    }
}
