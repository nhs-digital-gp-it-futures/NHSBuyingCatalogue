using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A Contact to whom an AssessmentMessage has been sent
  /// </summary>
  [Table(nameof(AssessmentMessageContact))]
  public sealed class AssessmentMessageContact
  {
    /// <summary>
    /// Unique identifier of AssessmentMessage
    /// </summary>
    [Required]
    [ExplicitKey]
    public string AssessmentMessageId { get; set; }

    /// <summary>
    /// Unique identifier of Contact
    /// </summary>
    [Required]
    [ExplicitKey]
    public string ContactId { get; set; }
  }
}
