using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
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
      var email = string.Empty;
      switch (context.UserName)
      {
        case Roles.Admin:
        case Roles.Buyer:
          primaryRoleId = PrimaryRole.GovernmentDepartment;
          email = "buying.catalogue.assessment@gmail.com";
          break;

        case Roles.Supplier:
          primaryRoleId = PrimaryRole.ApplicationServiceProvider;
          email = "buying.catalogue.supplier@gmail.com";
          break;

        default:
          break;
      }

      var servProv = context.HttpContext.RequestServices;
      var contStore = (IContactsDatastore)servProv.GetService(typeof(IContactsDatastore));
      var contact = contStore.ByEmail(email);
      var orgStore = (IOrganisationsDatastore)servProv.GetService(typeof(IOrganisationsDatastore));
      var org = orgStore.ByContact(contact.Id);
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email, context.Options.ClaimsIssuer),
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
