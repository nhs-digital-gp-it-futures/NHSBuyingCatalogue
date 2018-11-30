using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Primitives;
using NHSD.GPITF.BuyingCatalog.Authentications;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NHSD.GPITF.BuyingCatalog.Tests
{
  internal static class Creator
  {
    public static TokenValidatedContext GetTokenValidatedContext(string bearerToken)
    {
      var authScheme = new AuthenticationScheme("BearerAuthentication", "BearerAuthentication", typeof(DummyAuthenticationHandler));
      var options = new JwtBearerOptions();
      var ctx = new TokenValidatedContext(Creator.GetContext(bearerToken), authScheme, options)
      {
        Principal = new ClaimsPrincipal()
      };

      return ctx;
    }

    public static DummyHttpContext GetContext(string bearerToken)
    {
      var ctx = new DummyHttpContext();
      ((FrameRequestHeaders)ctx.Request.Headers).HeaderAuthorization = new StringValues(bearerToken);
      return ctx;
    }

    public static UserInfoResponse GetUserInfoResponse(
      IEnumerable<(string Type, string Value)> claims
      )
    {
      var jsonClaimsArray = claims.Select(c => $"{c.Type}:\"{c.Value}\"");
      var jsonClaims = "{" + string.Join(',', jsonClaimsArray) + "}";
      var response = new UserInfoResponse(jsonClaims);
      return response;
    }

    public static CachedUserInfoResponse GetCachedUserInfoResponseExpired(UserInfoResponse userInfoResponse)
    {
      return new CachedUserInfoResponse(userInfoResponse, new DateTime(2006, 2, 20));
    }

    // TODO   stole from NHSD.GPITF.BuyingCatalog.Logic.Tests\Creator.cs
    public static Contacts GetContact(
      string id = null,
      string orgId = null,
      string emailAddress1 = null)
    {
      var retval = new Contacts
      {
        Id = id ?? Guid.NewGuid().ToString(),
        OrganisationId = orgId ?? Guid.NewGuid().ToString(),
        EmailAddress1 = emailAddress1 ?? "jon.dough@tpp.com"
      };
      Verifier.Verify(retval);
      return retval;
    }

    // TODO   stole from NHSD.GPITF.BuyingCatalog.Logic.Tests\Creator.cs
    public static Organisations GetOrganisation(
      string id = "NHS Digital",
      string primaryRoleId = PrimaryRole.GovernmentDepartment,
      string odsCode = "NHS Digital ODS Code")
    {
      var retval = new Organisations
      {
        Id = id,
        Name = id,
        PrimaryRoleId = primaryRoleId,
        OdsCode = odsCode
      };
      Verifier.Verify(retval);
      return retval;
    }
  }
}
