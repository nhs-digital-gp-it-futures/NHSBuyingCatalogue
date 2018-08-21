namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// Status of a <see cref="StandardsApplicable"/> as it goes through various stages in its life cycle
  /// </summary>
  public enum ClaimedStandardStatus : int
    {
    /// <summary>
    /// <see cref="StandardsApplicable"/> has been submitted, for assessment by NHSD
    /// This is the starting point in the life cycle.
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// <see cref="StandardsApplicable"/> is being revised by <see cref="Organisations"/>
    /// </summary>
    Remediation = 1,

    /// <summary>
    /// The Evidence has reviewed by NHSD and the <see cref="StandardsApplicable"/> fulfills all
    /// of the requirements of the <see cref="Standards"/>
    /// This is a positive end point in the life cycle.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// The Evidence has reviewed by NHSD and the <see cref="StandardsApplicable"/> does NOT fulfill all
    /// of the requirements of the <see cref="Standards"/>
    /// This is a negative end point in the life cycle.
    /// An <see cref="Organisations"/> may remove the <see cref="StandardsApplicable"/>.
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// The Evidence has reviewed by NHSD and the <see cref="StandardsApplicable"/> fulfills enough
    /// of the requirements of the <see cref="Standards"/>
    /// This is a positive end point in the life cycle.
    /// </summary>
    PartiallyApproved = 4
  }
}
