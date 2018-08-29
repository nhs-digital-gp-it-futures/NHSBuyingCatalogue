using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A list of potential competencies a ‘solution’ can perform or provide eg
  /// * Mobile working
  /// * Training
  /// * Prescribing
  /// * Installation
  /// Note that a ‘capability’ has a link to zero or one previous ‘capability’
  /// Generally, only interested in current ‘capability’
  /// </summary>
  [Table(nameof(Capabilities))]
  public sealed class Capabilities
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
    /// Name of Capability, as displayed to a user
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of Capability, as displayed to a user
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// URL with further information
    /// </summary>
    public string URL { get; set; }
  }
}
