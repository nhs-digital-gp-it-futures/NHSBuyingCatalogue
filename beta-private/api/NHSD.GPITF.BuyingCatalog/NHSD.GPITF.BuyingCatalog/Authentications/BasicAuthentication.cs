using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace NHSD.GPITF.BuyingCatalog.Authentications
{
  internal static class BasicAuthentication
  {
    public static Task Authenticate(ValidatePrincipalContext context)
    {
      // use basic authentication to support Swagger
      if (context.UserName != context.Password)
      {
        context.AuthenticationFailMessage = "Authentication failed.";

        return Task.CompletedTask;
      }

      var primaryRoleId = string.Empty;
      switch (context.UserName)
      {
        case Roles.Admin:
        case Roles.Buyer:
          primaryRoleId = PrimaryRole.GovernmentDepartment;
          break;

        case Roles.Supplier:
          primaryRoleId = PrimaryRole.ApplicationServiceProvider;
          break;

        default:
          break;
      }

      var servProv = context.HttpContext.RequestServices;
      var orgStore = (IOrganisationsDatastore)servProv.GetService(typeof(IOrganisationsDatastore));
      var org = orgStore.GetAll().FirstOrDefault(x => x.PrimaryRoleId == primaryRoleId);
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer),

        // use (case-sensitive) UserName for role
        new Claim(ClaimTypes.Role, context.UserName),

        // random organisation for Joe public
        new Claim(nameof(Organisations), org?.Id ?? Guid.NewGuid().ToString())
      };

      context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, BasicAuthenticationDefaults.AuthenticationScheme));

      return Task.CompletedTask;
    }
  }
}
