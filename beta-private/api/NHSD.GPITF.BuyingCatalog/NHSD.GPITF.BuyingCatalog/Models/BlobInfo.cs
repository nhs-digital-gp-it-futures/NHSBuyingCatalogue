using System;

namespace NHSD.GPITF.BuyingCatalog.Models
{
#pragma warning disable CS1591
  public sealed class BlobInfo
  {
    /// <summary>
    /// Display name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// true if object is a folder
    /// </summary>
    bool IsFolder { get; }

    /// <summary>
    /// Externally accessible URL
    /// </summary>
    string Url { get; }

    /// <summary>
    /// UTC when last modified
    /// </summary>
    public DateTime TimeLastModified { get; }
  }
#pragma warning restore CS1591
}
