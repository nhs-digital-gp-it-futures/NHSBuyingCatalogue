using NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Notifier.Null
{
  public sealed class SolutionChangeNotifier : ISolutionsChangeNotifier
  {
    public void Notify(ChangeRecord<Solutions> record)
    {
    }
  }
}
