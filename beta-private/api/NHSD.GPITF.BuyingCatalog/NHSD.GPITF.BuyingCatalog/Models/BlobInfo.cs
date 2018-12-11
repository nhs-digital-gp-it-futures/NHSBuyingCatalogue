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
    /// size of file in bytes (zero for a folder)
    /// </summary>
    public long Length { get; set; }

    /// <summary>
    /// Externally accessible URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// UTC when last modified
    /// </summary>
    public DateTime TimeLastModified { get; set; }

    /// <summary>
    /// unique identifier of binary file in blob storage system
    /// (null for a folder)
    /// NOTE:  this may not be a GUID eg it may be a URL
    /// NOTE:  this is a GUID for SharePoint
    /// </summary>
    public string BlobId { get; set; }
  }
#pragma warning restore CS1591
}
