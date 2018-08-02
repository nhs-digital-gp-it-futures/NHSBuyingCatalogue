using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;
using System.Security.Claims;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public static class HttpContextAccessorExtensions
  {
    public static string ContextOrganisationId(this IHttpContextAccessor context)
    {
      return context.HttpContext.User.Claims
        .FirstOrDefault(x => x.Type == nameof(Organisation))?.Value;
    }

    public static bool HasRole(this IHttpContextAccessor context, string role)
    {
      return context.HttpContext.User.Claims
        .Where(x => x.Type == ClaimTypes.Role)
        .Select(x => x.Value)
        .Any(x => x == role);
    }
  }
}
