using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Models.Porcelain
{
  /// <summary>
  /// All CapabilityMapping with all corresponding Standard
  /// </summary>
  public sealed class CapabilityMappings
  {
    /// <summary>
    /// <see cref="CapabilityMapping"/>
    /// </summary>
    public List<CapabilityMapping> CapabilityMapping { get; set; } = new List<CapabilityMapping>();

    /// <summary>
    /// A list of <see cref="Standard"/>
    /// </summary>
    public List<Standard> Standard { get; set; } = new List<Standard>();
  }
}
