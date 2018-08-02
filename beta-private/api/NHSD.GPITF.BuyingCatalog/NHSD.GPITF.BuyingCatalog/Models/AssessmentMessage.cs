using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A message between Contacts in the context of a Solution,
  /// typically relating to assessment of the Solution
  /// </summary>
  [Table(nameof(AssessmentMessage))]
  public sealed class AssessmentMessage
  {
    /// <summary>
    /// Unique identifier of entity
    /// </summary>
    [Required]
    [ExplicitKey]
    public string Id { get; set; }

    /// <summary>
    /// Unique identifier of Solution
    /// </summary>
    [Required]
    public string SolutionId { get; set; }

    /// <summary>
    /// Unique identifier of Contact sending this message
    /// </summary>
    [Required]
    public string ContactId { get; set; }

    /// <summary>
    /// Server side (UTC) date and time this message was created
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Contents of message
    /// </summary>
    public string Message { get; set; }
  }
}
