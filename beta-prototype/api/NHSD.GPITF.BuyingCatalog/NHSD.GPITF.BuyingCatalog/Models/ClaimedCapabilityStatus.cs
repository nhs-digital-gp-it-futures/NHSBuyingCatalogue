namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// Status of a <see cref="ClaimedCapability"/> as it goes through various stages in its life cycle
  /// </summary>
  public enum ClaimedCapabilityStatus : int
  {
    /// <summary>
    /// <see cref="ClaimedCapability"/> has been submitted, for assessment by NHSD
    /// This is the starting point in the life cycle.
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// <see cref="ClaimedCapability"/> is being revised by <see cref="Organisation"/>
    /// </summary>
    Remediation = 1,

    /// <summary>
    /// The Evidence has reviewed by NHSD and the <see cref="ClaimedCapability"/> fulfills all
    /// of the requirements of the <see cref="Capability"/>
    /// This is a positive end point in the life cycle.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// The Evidence has reviewed by NHSD and the <see cref="ClaimedCapability"/> does NOT fulfill all
    /// of the requirements of the <see cref="Capability"/>
    /// This is a negative end point in the life cycle.
    /// An <see cref="Organisation"/> may remove the <see cref="ClaimedCapability"/>.
    /// </summary>
    Rejected = 3
  }
}
