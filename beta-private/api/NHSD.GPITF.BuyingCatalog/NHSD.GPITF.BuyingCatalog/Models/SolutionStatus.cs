namespace NHSD.GPITF.BuyingCatalog.Models
{
  /// <summary>
  /// Status of a solution as it goes through various stages in its life cycle
  /// </summary>
  public enum SolutionStatus : int
  {
    /// <summary>
    /// <see cref="Solution"/> is being revised by <see cref="Organisation"/>
    /// </summary>
    Draft = 0,

    /// <summary>
    /// <see cref="Solution"/> has been submitted by <see cref="Organisation"/>, for consideration
    /// by NHSD for inclusion into Buying Catalog
    /// </summary>
    Registered = 1,

    /// <summary>
    /// <see cref="Solution"/> has been assessed by NHSD and has been accepted onto the
    /// Buying Catalog.  The <see cref="Solution"/> will now have its <see cref="ClaimedCapability"/>
    /// assessed by NHSD.
    /// </summary>
    CapabilitiesAssessment = 2,

    /// <summary>
    /// <see cref="ClaimedCapability"/> have been verified by NHSD; and the <see cref="Solution"/>
    /// will now have its <see cref="ClaimedStandard"/> assessed by NHSD.
    /// </summary>
    StandardsCompliance = 3,

    /// <summary>
    /// <see cref="ClaimedStandard"/> have been verified by NHSD; and the <see cref="Organisation"/>
    /// will now build its solution page for the Buying Catalog.
    /// </summary>
    SolutionPage = 4,

    /// <summary>
    /// The solution page has reviewed by NHS and the <see cref="Solution"/> is now available for
    /// purchase on the Buying Catalog.
    /// </summary>
    Approved = 5,
  }
}
