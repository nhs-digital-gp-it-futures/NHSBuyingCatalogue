using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Configuration;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NHSD.GPITF.BuyingCatalog.Authentications
{
  internal static class BearerAuthentication
  {
    private static TimeSpan Expiry = TimeSpan.FromMinutes(60);

    // [bearerToken]-->[UserInfoResponse]
    private static Dictionary<string, CachedUserInfoResponse> _cache = new Dictionary<string, CachedUserInfoResponse>();

    private static object _cacheLock = new object();

    public static async Task Authenticate(IConfiguration config, TokenValidatedContext context)
    {
      // set roles based on email-->organisation-->org.PrimaryRoleId
      var bearerToken = ((FrameRequestHeaders)context.HttpContext.Request.Headers).HeaderAuthorization.Single();

      CachedUserInfoResponse cachedresponse;
      lock (_cacheLock)
      {
        // have to cache responses or UserInfo endpoint thinks we are a DOS attack
        if (_cache.TryGetValue(bearerToken, out cachedresponse))
        {
          if (cachedresponse.Created < DateTime.UtcNow.Subtract(Expiry))
          {
            _cache.Remove(bearerToken);
            cachedresponse = null;
          }
        }
      }

      var response = cachedresponse?.UserInfoResponse;
      if (response == null)
      {
        var userInfoClient = new UserInfoClient(config["Jwt:UserInfo"]);
        response = await userInfoClient.GetAsync(bearerToken.Substring(7));
        lock (_cacheLock)
        {
          _cache.Remove(bearerToken);
          _cache.Add(bearerToken, new CachedUserInfoResponse(response));
        }
      }

      var userClaims = response.Claims;
      var claims = new List<Claim>(userClaims);
      var email = userClaims.SingleOrDefault(x => x.Type == "email")?.Value;
      if (!string.IsNullOrEmpty(email))
      {
        var servProv = context.HttpContext.RequestServices;
        var contLog = (IContactsDatastore)servProv.GetService(typeof(IContactsDatastore));
        var contact = contLog.ByEmail(email);
        var orgLog = (IOrganisationsDatastore)servProv.GetService(typeof(IOrganisationsDatastore));
        var org = orgLog.ById(contact.OrganisationId);
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

    private sealed class CachedUserInfoResponse
    {
      public UserInfoResponse UserInfoResponse { get; }
      public DateTime Created { get; } = DateTime.UtcNow;

      public CachedUserInfoResponse(UserInfoResponse userInfoResponse)
      {
        UserInfoResponse = userInfoResponse;
      }
    }
  }
}

