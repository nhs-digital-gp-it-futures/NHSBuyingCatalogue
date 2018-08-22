using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// Initially, a 'message' or response to 'evidence' which supports a claim to a ‘capability’.
  /// Thereafter, this will be a response to a previous message/response.
  /// </summary>
  [Table(nameof(CapabilitiesImplementedReviews))]
  public sealed class CapabilitiesImplementedReviews
  {
    /// <summary>
    /// Unique identifier of entity
    /// </summary>
    [Required]
    [ExplicitKey]
    public string Id { get; set; }

    /// <summary>
    /// Unique identifier of CapabilitiesImplementedEvidence
    /// </summary>
    [Required]
    public string CapabilitiesImplementedEvidenceId { get; set; }

    /// <summary>
    /// Unique identifier of Contact who created record
    /// Derived from calling context
    /// SET ON SERVER
    /// </summary>
    [Required]
    public string CreatedById { get; set; }

    /// <summary>
    /// UTC date and time at which record created
    /// Set by server when creating record
    /// SET ON SERVER
    /// </summary>
    [Required]
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Serialised message data
    /// </summary>
    public string Message { get; set; } = string.Empty;
  }
}
