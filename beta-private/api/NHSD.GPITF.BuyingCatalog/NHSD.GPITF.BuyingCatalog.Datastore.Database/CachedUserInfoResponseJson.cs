using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  [Table(nameof(CachedUserInfoResponseJson))]
  public sealed class CachedUserInfoResponseJson
  {
    /// <summary>
    /// Bearer token
    /// </summary>
    [Required]
    public string Id { get; set; }

    /// <summary>
    /// JSON serialised CachedUserInfoResponse
    /// </summary>
    public string Data { get; set; }
  }
}
