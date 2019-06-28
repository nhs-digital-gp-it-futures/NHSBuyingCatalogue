using NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Notifier.Null
{
  public sealed class SolutionChangeNotifier : ISolutionsChangeNotifier
  {
    public void Notify(ChangeRecord<Models.Solutions> record)
    {
    }
  }
}
