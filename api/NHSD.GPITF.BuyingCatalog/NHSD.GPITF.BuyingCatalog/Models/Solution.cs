using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A product and/or service provided by an ‘organisation’.
  /// Note that a ‘solution’ has a link to zero or one previous ‘solution’
  /// Generally, only interested in current ‘solution’
  /// Standard MS Dynamics CRM entity
  /// </summary>
  [Table(nameof(Solution))]
  public sealed class Solution
  {
    /// <summary>
    /// Unique identifier of entity
    /// </summary>
    [Required]
    [ExplicitKey]
    public string Id { get; set; }

    /// <summary>
    /// Unique identifier of organisation
    /// </summary>
    [Required]
    public string OrganisationId { get; set; }

    /// <summary>
    /// Version of solution
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Current status of this solution
    /// </summary>
    public SolutionStatus Status { get; set; } = SolutionStatus.Draft;

    /// <summary>
    /// Name of solution, as displayed to a user
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of solution, as displayed to a user
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Serialised product page
    /// </summary>
    public string ProductPage { get; set; } = string.Empty;
  }
}
