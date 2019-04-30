using Microsoft.SharePoint.Client.NetCore;

namespace NHSD.GPITF.BuyingCatalog
{
#pragma warning disable CS0414
#pragma warning disable S1144 // Unused private types or members should be removed

  internal sealed class SharePointIncluder
  {
    // This is a workaround to force loading of Microsoft.SharePoint.Client.NetCore
    // assembly.  If this is not done, then Microsoft.SharePoint.Client.NetCore
    // will not be able to unmarshal json from SharePoint to strongly typed objects.
    // Go figure...
    private static readonly AlertType dummy = AlertType.Custom;

    private SharePointIncluder()
    {
    }
  }

#pragma warning restore S1144 // Unused private types or members should be removed
#pragma warning restore CS0414
}
