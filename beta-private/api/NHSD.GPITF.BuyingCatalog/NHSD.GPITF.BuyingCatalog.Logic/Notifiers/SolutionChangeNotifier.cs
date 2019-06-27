using Microsoft.Extensions.Configuration;
using NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic.Notifiers
{
  public sealed class SolutionChangeNotifier : ChangeNotifierBase<Solutions>, ISolutionsChangeNotifier
  {
    public SolutionChangeNotifier(IConfiguration config) :
      base(config)
    {
    }
  }
}
