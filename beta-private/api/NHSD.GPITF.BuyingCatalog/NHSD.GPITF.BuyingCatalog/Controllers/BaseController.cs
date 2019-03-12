using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
#pragma warning disable CS1591
  public abstract class BaseController : ControllerBase
  {
    protected static object _syncRoot = new object();
  }
#pragma warning restore CS1591
}
