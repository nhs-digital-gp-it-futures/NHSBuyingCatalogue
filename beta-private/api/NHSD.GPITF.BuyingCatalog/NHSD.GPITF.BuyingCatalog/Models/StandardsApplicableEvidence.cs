﻿using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// A piece of 'evidence' which supports a claim to a ‘standard’.
  /// This is then assessed by NHS to verify the ‘solution’ complies with the ‘standard’ it has claimed.
  /// </summary>
  [Table(nameof(StandardsApplicableEvidence))]
  public class StandardsApplicableEvidence
  {
    /// <summary>
    /// Unique identifier of entity
    /// </summary>
    [Required]
    [ExplicitKey]
    public string Id { get; set; }

    /// <summary>
    /// Unique identifier of StandardsApplicable
    /// </summary>
    [Required]
    public string StandardsApplicableId { get; set; }

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
    /// Serialised evidence data
    /// </summary>
    public string Evidence { get; set; } = string.Empty;
  }
}