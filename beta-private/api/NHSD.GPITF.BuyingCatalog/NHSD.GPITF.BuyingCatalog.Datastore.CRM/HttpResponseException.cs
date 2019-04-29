using System;
using System.Net;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
  public sealed class HttpResponseException : Exception
  {
    public HttpStatusCode StatusCode { get; }

    public HttpResponseException(HttpStatusCode statusCode, string message) :
      base(message)
    {
      StatusCode = statusCode;
    }
  }
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
}
