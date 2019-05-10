using System;
using System.IO;
using System.Net;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
#pragma warning disable S2933 // Fields that are only assigned in the constructor should be "readonly"

  public sealed class OAuth2WebRequest : WebRequest
  {
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10.0);

    private WebRequest _innerRequest;

    private OAuth2AccessTokenRequest _request;

    public OAuth2WebRequest(string requestUriString, OAuth2AccessTokenRequest request)
    {
      _innerRequest = Create(requestUriString);
      _request = request;
    }

    public override WebResponse GetResponse()
    {
      string text = _request.ToString();
      _innerRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
      _innerRequest.ContentLength = text.Length;
      _innerRequest.ContentType = "application/x-www-form-urlencoded";
      _innerRequest.Method = "POST";
      _innerRequest.Timeout = (int)DefaultTimeout.TotalMilliseconds;
      StreamWriter streamWriter = new StreamWriter(_innerRequest.GetRequestStream(), System.Text.Encoding.ASCII);
      streamWriter.Write(text);
      streamWriter.Close();

      return _innerRequest.GetResponse();
    }
  }

#pragma warning restore S2933 // Fields that are only assigned in the constructor should be "readonly"
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
}
