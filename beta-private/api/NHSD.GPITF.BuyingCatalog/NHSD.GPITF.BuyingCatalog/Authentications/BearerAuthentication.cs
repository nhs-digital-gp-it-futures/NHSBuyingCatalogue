using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NHSD.GPITF.BuyingCatalog.Authentications
{
#pragma warning disable CS1591
  public static partial class BearerAuthentication
  {
    private static TimeSpan Expiry = TimeSpan.FromMinutes(60);

    public static async Task Authenticate(
      IUserInfoResponseDatastore cache,
      IConfiguration config,
      IUserInfoResponseRetriever userInfoClient,
      IContactsDatastore contactsDatastore,
      IOrganisationsDatastore organisationDatastore,
      TokenValidatedContext context)
    {
      // set roles based on email-->organisation-->org.PrimaryRoleId
      var bearerToken = ((FrameRequestHeaders)context.HttpContext.Request.Headers).HeaderAuthorization.Single();

      // have to cache responses or UserInfo endpoint thinks we are a DOS attack
      CachedUserInfoResponse cachedresponse = null;
      if (cache.TryGetValue(bearerToken, out string jsonCachedResponse))
      {
        cachedresponse = JsonConvert.DeserializeObject<CachedUserInfoResponse>(jsonCachedResponse);
        if (cachedresponse.Created < DateTime.UtcNow.Subtract(Expiry))
        {
          cache.Remove(bearerToken);
          cachedresponse = null;
        }
      }

      var response = cachedresponse?.UserInfoResponse;
      if (response == null)
      {
        var userInfo = Environment.GetEnvironmentVariable("OIDC_USERINFO_URL") ?? config["Jwt:UserInfo"];
        response = await userInfoClient.GetAsync(userInfo, bearerToken.Substring(7));
        if (response == null)
        {
          return;
        }
        cache.Remove(bearerToken);
        cache.Add(bearerToken, JsonConvert.SerializeObject(new CachedUserInfoResponse(response)));
      }

      if (response?.Claims == null)
      {
        return;
      }

      var userClaims = response.Claims;
      var claims = new List<Claim>(userClaims);
      var email = userClaims.SingleOrDefault(x => x.Type == "email")?.Value;
      if (!string.IsNullOrEmpty(email))
      {        var contact = contactsDatastore.ByEmail(email);

        if (contact == null)
        {
          return;
        }

        var org = organisationDatastore.ByContact(contact.Id);
        if (org == null)
        {
          return;
        }

        switch (org.PrimaryRoleId)
        {
          case PrimaryRole.ApplicationServiceProvider:
            claims.Add(new Claim(ClaimTypes.Role, Roles.Supplier));
            break;

          case PrimaryRole.GovernmentDepartment:
            claims.Add(new Claim(ClaimTypes.Role, Roles.Admin));
            claims.Add(new Claim(ClaimTypes.Role, Roles.Buyer));
            break;
        }
        claims.Add(new Claim(nameof(Organisations), org.Id));
      }

      context.Principal.AddIdentity(new ClaimsIdentity(claims));
    }
  }
#pragma warning restore CS1591
}

