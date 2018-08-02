using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A typically, optional ‘Standard’ which a ‘Solution’ asserts that it provides
  /// as part of providing a 'Capability'
  /// This is then assessed by NHS to verify the ‘Solution’ complies with the ‘Standard’ it has claimed.
  /// </summary>
  [Table(nameof(ClaimedCapabilityStandard))]
  public sealed class ClaimedCapabilityStandard
  {
    /// <summary>
    /// Unique identifier of ClaimedCapability
    /// </summary>
    [Required]
    [ExplicitKey]
    public string ClaimedCapabilityId { get; set; }

    /// <summary>
    /// Unique identifier of Standard
    /// </summary>
    [Required]
    [ExplicitKey]
    public string StandardId { get; set; }

    /// <summary>
    /// Serialised evidence data
    /// </summary>
    public string Evidence { get; set; } = string.Empty;

    /// <summary>
    /// Current status of this ClaimedCapabilityStandard
    /// </summary>
    public ClaimedStandardStatus Status { get; set; } = ClaimedStandardStatus.Submitted;
  }
}
