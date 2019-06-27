using NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHSD.GPITF.BuyingCatalog.Logic.Notifiers
{
  public sealed class SolutionChangeNotifier : ISolutionsChangeNotifier
  {
    public void Notify(ChangeRecord<Solutions> record)
    {
      throw new NotImplementedException();
    }
  }
}
