using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;
using System.Security.Claims;

#pragma warning disable CS1591
namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public static class HttpContextAccessorExtensions
  {
    public static string ContextOrganisationId(this IHttpContextAccessor context)
    {
      return context.HttpContext.User.Claims
        .FirstOrDefault(x => x.Type == nameof(Organisations))?.Value;
    }

    public static string ContextEmail(this IHttpContextAccessor context)
    {
      return context.HttpContext.User.Claims
        .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
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
#pragma warning restore CS1591
