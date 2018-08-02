using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A ‘Standard’ which a ‘Solution’ asserts that it provides.
  /// This is then assessed by NHS to verify the ‘Solution’ complies with the ‘Standard’ it has claimed.
  /// </summary>
  [Table(nameof(ClaimedStandard))]
  public sealed class ClaimedStandard
  {
    /// <summary>
    /// Unique identifier of entity
    /// </summary>
    [Required]
    [ExplicitKey]
    public string Id { get; set; }

    /// <summary>
    /// Unique identifier of solution
    /// </summary>
    [Required]
    public string SolutionId { get; set; }

    /// <summary>
    /// Unique identifier of standard
    /// </summary>
    [Required]
    public string StandardId { get; set; }

    /// <summary>
    /// Serialised evidence data
    /// </summary>
    public string Evidence { get; set; } = string.Empty;

    /// <summary>
    /// Current status of this ClaimedStandard
    /// </summary>
    public ClaimedStandardStatus Status { get; set; } = ClaimedStandardStatus.Submitted;
  }
}
