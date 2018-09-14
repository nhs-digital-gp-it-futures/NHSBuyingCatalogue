using IdentityModel.Client;
using System;

namespace NHSD.GPITF.BuyingCatalog.Authentications
{
#pragma warning disable CS1591
  public sealed class CachedUserInfoResponse
  {
    public UserInfoResponse UserInfoResponse { get; }
    public DateTime Created { get; set; } = DateTime.UtcNow;

    // required for Json constructor
    public CachedUserInfoResponse()
    {
    }

    public CachedUserInfoResponse(UserInfoResponse userInfoResponse, DateTime created) :
      this(userInfoResponse)
    {
      Created = created;
    }

    public CachedUserInfoResponse(UserInfoResponse userInfoResponse)
    {
      UserInfoResponse = userInfoResponse;
    }
  }
#pragma warning restore CS1591
}

