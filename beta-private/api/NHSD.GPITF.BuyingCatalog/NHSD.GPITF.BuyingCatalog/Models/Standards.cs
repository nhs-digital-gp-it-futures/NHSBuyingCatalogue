using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A formal set of requirements eg  * ISO 9001
  /// Note that a ‘standard’ has a link to zero or one previous ‘standard’
  /// Generally, only interested in current ‘standard’
  /// </summary>
  [Table(nameof(Standards))]
  public sealed class Standards
  {
    /// <summary>
    /// Unique identifier of entity
    /// </summary>
    [Required]
    [ExplicitKey]
    public string Id { get; set; }

    /// <summary>
    /// Unique identifier of previous version of entity
    /// </summary>
    public string PreviousId { get; set; }

    /// <summary>
    /// True if this standard applies to all solutions
    /// </summary>
    public bool IsOverarching { get; set; }

    /// <summary>
    /// Name of Standard, as displayed to a user
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of Standard, as displayed to a user
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// URL with further information
    /// </summary>
    public string URL { get; set; }
  }
}
