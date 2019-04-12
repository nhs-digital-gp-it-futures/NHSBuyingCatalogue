using System;
using System.Net;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class HttpResponseException : Exception
  {
    public HttpStatusCode StatusCode { get; }

    public HttpResponseException(HttpStatusCode statusCode, string message) :
      base(message)
    {
      StatusCode = statusCode;
    }
  }
}
