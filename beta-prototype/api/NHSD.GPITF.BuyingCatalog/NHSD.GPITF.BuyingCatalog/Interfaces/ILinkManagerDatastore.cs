namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ILinkManagerDatastore
  {
    void CapabilityFrameworkCreate(string frameworkId, string capabilityId);
    void CapabilityStandardCreate(string capabilityId, string standardId, bool isOptional);
    void FrameworkSolutionCreate(string frameworkId, string solutionId);
    void FrameworkStandardCreate(string frameworkId, string standardId);
  }
#pragma warning restore CS1591
}
