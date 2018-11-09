using Microsoft.Extensions.Configuration;
using Microsoft.SharePoint.Client.NetCore;
using Microsoft.SharePoint.Client.NetCore.Runtime;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public sealed class EvidenceBlobStoreDatastore : IEvidenceBlobStoreDatastore
  {
    private readonly ClientContext _context;

    private readonly IOrganisationsDatastore _organisationsDatastore;
    private readonly ISolutionsDatastore _solutionsDatastore;
    private readonly ICapabilitiesImplementedDatastore _capabilitiesImplementedDatastore;
    private readonly IStandardsApplicableDatastore _standardsApplicableDatastore;
    private readonly ICapabilitiesDatastore _capabilitiesDatastore;
    private readonly IStandardsDatastore _standardsDatastore;

    private readonly string SharePoint_BaseUrl;
    private readonly string SharePoint_OrganisationsRelativeUrl;
    private readonly string SharePoint_Login;
    private readonly string SharePoint_Password;

    public EvidenceBlobStoreDatastore(
      IConfiguration config,
      IOrganisationsDatastore organisationsDatastore,
      ISolutionsDatastore solutionsDatastore,
      ICapabilitiesImplementedDatastore capabilitiesImplementedDatastore,
      IStandardsApplicableDatastore standardsApplicableDatastore,
      ICapabilitiesDatastore capabilitiesDatastore,
      IStandardsDatastore standardsDatastore
      )
    {
      _solutionsDatastore = solutionsDatastore;
      _organisationsDatastore = organisationsDatastore;
      _capabilitiesImplementedDatastore = capabilitiesImplementedDatastore;
      _standardsApplicableDatastore = standardsApplicableDatastore;
      _capabilitiesDatastore = capabilitiesDatastore;
      _standardsDatastore = standardsDatastore;

      SharePoint_BaseUrl = Environment.GetEnvironmentVariable("SHAREPOINT_BASEURL") ?? config["SharePoint:BaseUrl"];
      SharePoint_OrganisationsRelativeUrl = Environment.GetEnvironmentVariable("SHAREPOINT_ORGANISATIONSRELATIVEURL") ?? config["SharePoint:OrganisationsRelativeUrl"];
      SharePoint_Login = Environment.GetEnvironmentVariable("SHAREPOINT_LOGIN") ?? config["SharePoint:Login"];
      SharePoint_Password = Environment.GetEnvironmentVariable("SHAREPOINT_PASSWORD") ?? config["SharePoint:Password"];

      var securePassword = new SecureString();
      foreach (char item in SharePoint_Password)
      {
        securePassword.AppendChar(item);
      }

      var encodedUrl = Uri.EscapeUriString(SharePoint_BaseUrl);
      var onlineCredentials = new SharePointOnlineCredentials(SharePoint_Login, securePassword);
      _context = new ClientContext(encodedUrl)
      {
        Credentials = onlineCredentials
      };
    }

    public string AddEvidenceForClaim(IClaimsInfoProvider claimsInfoProvider, string claimId, Stream file, string fileName, string subFolder = null)
    {
      return UploadFileSlicePerSlice(claimsInfoProvider, claimId, file, fileName, subFolder);
    }

    private string UploadFileSlicePerSlice(IClaimsInfoProvider claimsInfoProvider, string claimId, Stream file, string fileName, string subFolder, int fileChunkSizeInMB = 3)
    {
      // Each sliced upload requires a unique id
      var uploadId = Guid.NewGuid();

      // Get to folder to upload into
      var claim = claimsInfoProvider.GetClaimById(claimId);
      var soln = _solutionsDatastore.ById(claim.SolutionId);
      var org = _organisationsDatastore.ById(soln.OrganisationId);
      var subFolderSeparator = !string.IsNullOrEmpty(subFolder) ? "/" : string.Empty;
      var claimFolder = $"{SharePoint_BaseUrl}/{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{claimsInfoProvider.GetFolderName()}/{claimsInfoProvider.GetFolderClaimName(claim)}";
      var absUri = new Uri($"{claimFolder}/{subFolder ?? string.Empty}{subFolderSeparator}{fileName}");
      var claimFolderRelUrl = $"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{claimsInfoProvider.GetFolderName()}/{claimsInfoProvider.GetFolderClaimName(claim)}/{subFolder ?? string.Empty}{subFolderSeparator}";

      // create subFolder if not exists
      if (!string.IsNullOrEmpty(subFolder))
      {
        CreateSubFolder(claimFolder, subFolder);
      }

      var docClaimFolder = _context.Web.GetFolderByServerRelativeUrl(claimFolderRelUrl);
      _context.ExecuteQuery();

      // Get the information about the folder that will hold the file
      _context.Load(docClaimFolder.Files);
      _context.Load(docClaimFolder, folder => folder.ServerRelativeUrl);
      _context.ExecuteQuery();

      using (var br = new BinaryReader(file))
      {
        var fileSize = file.Length;
        ClientResult<long> bytesUploaded = null;
        Microsoft.SharePoint.Client.NetCore.File uploadFile = null;

        // Calculate block size in bytes
        var blockSize = fileChunkSizeInMB * 1024 * 1024;

        byte[] buffer = new byte[blockSize];
        byte[] lastBuffer = null;
        long fileoffset = 0;
        long totalBytesRead = 0;
        int bytesRead;
        bool first = true;
        bool last = false;

        // Read data from stream in blocks 
        while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
        {
          totalBytesRead += bytesRead;

          // We've reached the end of the file
          if (totalBytesRead == fileSize)
          {
            last = true;

            // Copy to a new buffer that has the correct size
            lastBuffer = new byte[bytesRead];
            Array.Copy(buffer, 0, lastBuffer, 0, bytesRead);
          }

          if (first)
          {
            using (var contentStream = new MemoryStream())
            {
              // Add an empty file.
              var fileInfo = new FileCreationInformation
              {
                ContentStream = contentStream,
                Url = fileName,
                Overwrite = true
              };
              uploadFile = docClaimFolder.Files.Add(fileInfo);

              // Start upload by uploading the first slice
              using (var strm = new MemoryStream(buffer))
              {
                // Call the start upload method on the first slice
                bytesUploaded = uploadFile.StartUpload(uploadId, strm);
                _context.ExecuteQuery();

                // fileoffset is the pointer where the next slice will be added
                fileoffset = bytesUploaded.Value;
              }

              // we can only start the upload once
              first = false;
            }
          }

          // Get a reference to our file
          uploadFile = _context.Web.GetFileByServerRelativeUrl(docClaimFolder.ServerRelativeUrl + Path.AltDirectorySeparatorChar + fileName);

          if (last)
          {
            // Is this the last slice of data?
            using (var strm = new MemoryStream(lastBuffer))
            {
              // End sliced upload by calling FinishUpload
              uploadFile = uploadFile.FinishUpload(uploadId, fileoffset, strm);
              _context.ExecuteQuery();

              // return the url for the uploaded file
              return absUri.AbsoluteUri;
            }
          }

          using (var strm = new MemoryStream(buffer))
          {
            // Continue sliced upload
            bytesUploaded = uploadFile.ContinueUpload(uploadId, fileoffset, strm);
            _context.ExecuteQuery();

            // update fileoffset for the next slice
            fileoffset = bytesUploaded.Value;
          }
        }
      }

      return string.Empty;
    }

    public IEnumerable<BlobInfo> EnumerateFolder(IClaimsInfoProvider claimsInfoProvider, string claimId, string subFolder = null)
    {
      var claim = claimsInfoProvider.GetClaimById(claimId);
      var soln = _solutionsDatastore.ById(claim.SolutionId);
      var org = _organisationsDatastore.ById(soln.OrganisationId);

      var claimFolderUrl = $"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{claimsInfoProvider.GetFolderName()}/{claimsInfoProvider.GetFolderClaimName(claim)}/{subFolder ?? string.Empty}";
      var claimFolder = _context.Web.GetFolderByServerRelativeUrl(Uri.EscapeUriString(claimFolderUrl));

      _context.Load(claimFolder);
      _context.Load(claimFolder.Files);
      _context.Load(claimFolder.Folders);
      try
      {
        _context.ExecuteQuery();
      }
      catch (Exception ex)
      {
        throw new KeyNotFoundException($"Folder does not exist!: {_context.Url}/{claimFolderUrl}", ex);
      }

      var claimFolderInfo = new BlobInfo
      {
        Name = claimFolder.Name,
        IsFolder = true,
        Url = new Uri(new Uri(_context.Url), claimFolder.ServerRelativeUrl).AbsoluteUri,
        TimeLastModified = claimFolder.TimeLastModified
      };
      var claimSubFolderInfos = claimFolder
        .Folders
        .Select(x =>
          new BlobInfo
          {
            Name = x.Name,
            IsFolder = true,
            Url = new Uri(new Uri(_context.Url), x.ServerRelativeUrl).AbsoluteUri,
            TimeLastModified = x.TimeLastModified
          });
      var claimFileInfos = claimFolder
        .Files
        .Select(x =>
          new BlobInfo
          {
            Name = x.Name,
            IsFolder = false,
            Url = new Uri(new Uri(_context.Url), x.ServerRelativeUrl).AbsoluteUri,
            TimeLastModified = x.TimeLastModified
          });
      var retVal = new List<BlobInfo>();

      retVal.Add(claimFolderInfo);
      retVal.AddRange(claimSubFolderInfos);
      retVal.AddRange(claimFileInfos);

      return retVal;
    }

    public Stream GetFileStream(IClaimsInfoProvider claimsInfoProvider, string claimId, string extUrl)
    {
      var serverRelURL = new Uri(extUrl).AbsolutePath;
      return Microsoft.SharePoint.Client.NetCore.File.OpenBinaryDirect(_context, serverRelURL)?.Stream;
    }

    public void PrepareForSolution(IClaimsInfoProvider claimsInfoProvider, string solutionId)
    {
      var soln = _solutionsDatastore.ById(solutionId);
      if (soln == null)
      {
        throw new KeyNotFoundException($"Could not find solution: {solutionId}");
      }
      var org = _organisationsDatastore.ById(soln.OrganisationId);
      var claimedCapNames = _capabilitiesImplementedDatastore
        .BySolution(solutionId)
        .Select(x => _capabilitiesDatastore.ById(x.CapabilityId).Name);
      var claimedNameStds = _standardsApplicableDatastore
        .BySolution(solutionId)
        .Select(x => _standardsDatastore.ById(x.StandardId).Name);

      CreateSubFolder(SharePoint_OrganisationsRelativeUrl, org.Name);
      CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}", soln.Name);
      CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}", claimsInfoProvider.GetCapabilityFolderName());
      CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}", claimsInfoProvider.GetStandardsFolderName());
      foreach (var folderName in claimedCapNames)
      {
        CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{claimsInfoProvider.GetCapabilityFolderName()}", folderName);
      }
      foreach (var folderName in claimedNameStds)
      {
        CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{claimsInfoProvider.GetStandardsFolderName()}", folderName);
      }
    }

    // can only create sub-folder immediately under baseUrl
    private void CreateSubFolder(string baseUrl, string subFolder)
    {
      var baseFolderExists = true;
      var baseFolder = _context.Web.GetFolderByServerRelativeUrl(Uri.EscapeUriString($"{baseUrl}"));
      _context.Load(baseFolder);
      try
      {
        _context.ExecuteQuery();
      }
      catch
      {
        baseFolderExists = false;
      }
      if (!baseFolderExists)
      {
        return;
      }

      var targetFolderExists = true;
      var targetFolder = _context.Web.GetFolderByServerRelativeUrl(Uri.EscapeUriString($"{baseUrl}/{subFolder}"));
      _context.Load(targetFolder);
      try
      {
        _context.ExecuteQuery();
      }
      catch
      {
        targetFolderExists = false;
      }
      if (targetFolderExists)
      {
        return;
      }

      baseFolder.AddSubFolder(subFolder);
      _context.ExecuteQuery();
    }
  }
}
