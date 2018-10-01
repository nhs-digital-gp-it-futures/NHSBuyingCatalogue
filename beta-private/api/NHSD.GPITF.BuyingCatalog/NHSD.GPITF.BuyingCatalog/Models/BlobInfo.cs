using System;

namespace NHSD.GPITF.BuyingCatalog.Models
{
#pragma warning disable CS1591
  public sealed class BlobInfo
  {
    /// <summary>
    /// Display name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// true if object is a folder
    /// </summary>
    public bool IsFolder { get; set; }

    /// <summary>
    /// Externally accessible URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// UTC when last modified
    /// </summary>
    public DateTime TimeLastModified { get; set; }
  }
#pragma warning restore CS1591
}
