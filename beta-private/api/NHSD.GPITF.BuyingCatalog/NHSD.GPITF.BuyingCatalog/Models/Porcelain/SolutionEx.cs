using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Models.Porcelain
{
  /// <summary>
  /// An Extended Solution with its corresponding Technical Contacts, ClaimedCapability, ClaimedStandard et al
  /// </summary>
  public sealed class SolutionEx
  {
    /// <summary>
    /// Solution
    /// </summary>
    public Solution Solution { get; set; }

    /// <summary>
    /// A list of ClaimedCapability
    /// </summary>
    public List<ClaimedCapability> ClaimedCapability { get; set; } = new List<ClaimedCapability>();

    /// <summary>
    /// A list of ClaimedStandard
    /// </summary>
    public List<ClaimedStandard> ClaimedStandard { get; set; } = new List<ClaimedStandard>();

    /// <summary>
    /// A list of TechnicalContact
    /// </summary>
    public List<TechnicalContact> TechnicalContact { get; set; } = new List<TechnicalContact>();

    /// <summary>
    /// A list of ClaimedCapabilityStandard
    /// </summary>
    public List<ClaimedCapabilityStandard> ClaimedCapabilityStandard { get; set; } = new List<ClaimedCapabilityStandard>();
  }
}
