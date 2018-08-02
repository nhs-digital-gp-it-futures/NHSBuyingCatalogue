namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// Status of a <see cref="ClaimedStandard"/> as it goes through various stages in its life cycle
  /// </summary>
  public enum ClaimedStandardStatus : int
    {
    /// <summary>
    /// <see cref="ClaimedStandard"/> has been submitted, for assessment by NHSD
    /// This is the starting point in the life cycle.
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// <see cref="ClaimedStandard"/> is being revised by <see cref="Organisation"/>
    /// </summary>
    Remediation = 1,

    /// <summary>
    /// The Evidence has reviewed by NHSD and the <see cref="ClaimedStandard"/> fulfills all
    /// of the requirements of the <see cref="Standard"/>
    /// This is a positive end point in the life cycle.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// The Evidence has reviewed by NHSD and the <see cref="ClaimedStandard"/> does NOT fulfill all
    /// of the requirements of the <see cref="Standard"/>
    /// This is a negative end point in the life cycle.
    /// An <see cref="Organisation"/> may remove the <see cref="ClaimedStandard"/>.
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// The Evidence has reviewed by NHSD and the <see cref="ClaimedStandard"/> fulfills enough
    /// of the requirements of the <see cref="Standard"/>
    /// This is a positive end point in the life cycle.
    /// </summary>
    PartiallyApproved = 4
  }
}
