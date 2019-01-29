using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
#pragma warning disable CS1591
  public abstract class ClaimsBase
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
    /// Unique identifier of supplier Contact who is responsible for this claim
    /// </summary>
    public string OwnerId { get; set; }

    /// <summary>
    /// Unique identifier of Capability or Standard
    /// </summary>
    public abstract string QualityId { get; }
  }
#pragma warning restore CS1591
}
