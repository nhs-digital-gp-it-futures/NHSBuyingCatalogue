using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A ‘Standard’ which a ‘Solution’ asserts that it provides.
  /// This is then assessed by NHS to verify the ‘Solution’ complies with the ‘Standard’ it has claimed.
  /// </summary>
  [Table(nameof(StandardsApplicable))]
  public sealed class StandardsApplicable : ClaimsBase
  {
    /// <summary>
    /// Unique identifier of standard
    /// </summary>
    [Required]
    public string StandardId { get; set; }

    /// <summary>
    /// Current status of this ClaimedStandard
    /// </summary>
    public StandardsApplicableStatus Status { get; set; } = StandardsApplicableStatus.Submitted;

    /// <summary>
    /// Unique identifier of Standard
    /// </summary>
    public override string QualityId => StandardId;
  }
}
