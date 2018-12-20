using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SharePoint.Client.NetCore;
using Microsoft.SharePoint.Client.NetCore.Runtime;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using Polly;
using System;
using System.Collections.Generic;
using System.Configuration;
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

    private readonly ILogger<IEvidenceBlobStoreDatastore> _logger;
    private readonly ISyncPolicy _policy;

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
      IStandardsDatastore standardsDatastore,
      ILogger<IEvidenceBlobStoreDatastore> logger,
      ISyncPolicyFactory policy
      )
    {
      _solutionsDatastore = solutionsDatastore;
      _organisationsDatastore = organisationsDatastore;
      _capabilitiesImplementedDatastore = capabilitiesImplementedDatastore;
      _standardsApplicableDatastore = standardsApplicableDatastore;
      _capabilitiesDatastore = capabilitiesDatastore;
      _standardsDatastore = standardsDatastore;

      _logger = logger;
      _policy = policy.Build(_logger);

      SharePoint_BaseUrl = Environment.GetEnvironmentVariable("SHAREPOINT_BASEURL") ?? config["SharePoint:BaseUrl"];
      SharePoint_OrganisationsRelativeUrl = Environment.GetEnvironmentVariable("SHAREPOINT_ORGANISATIONSRELATIVEURL") ?? config["SharePoint:OrganisationsRelativeUrl"];
      SharePoint_Login = Environment.GetEnvironmentVariable("SHAREPOINT_LOGIN") ?? config["SharePoint:Login"];
      SharePoint_Password = Environment.GetEnvironmentVariable("SHAREPOINT_PASSWORD") ?? config["SharePoint:Password"];

      if (string.IsNullOrWhiteSpace(SharePoint_BaseUrl) ||
        string.IsNullOrWhiteSpace(SharePoint_OrganisationsRelativeUrl) ||
        string.IsNullOrWhiteSpace(SharePoint_Login) ||
        string.IsNullOrWhiteSpace(SharePoint_Password)
        )
      {
        throw new ConfigurationErrorsException("Missing SharePoint configuration - check UserSecrets or environment variables");
      }

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
      return GetInternal(() =>
      {
        _logger.LogInformation($"AddEvidenceForClaim: claimId: {claimId} | fileName: {fileName} | subFolder: {subFolder}");
        return UploadFileSlicePerSlice(claimsInfoProvider, claimId, file, fileName, subFolder);
      });
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
      var claimFolderRelUrl = $"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{claimsInfoProvider.GetFolderName()}/{claimsInfoProvider.GetFolderClaimName(claim)}/{subFolder ?? string.Empty}{subFolderSeparator}";

      // create subFolder if not exists
      if (!string.IsNullOrEmpty(subFolder))
      {
        CreateSubFolder(claimFolder, subFolder);
      }

      var docClaimFolder = _context.Web.GetFolderByServerRelativeUrl(claimFolderRelUrl);
      _context.ExecuteQuery();

      // Get the information about the folder that will hold the file
      _logger.LogInformation($"UploadFileSlicePerSlice: enumerating {_context.Url}/{claimFolderRelUrl}...");
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
              // NOTE:  small files will be contained in the lastBuffer, so use this to upload in one call
              using (var strm = new MemoryStream(last ? lastBuffer : buffer))
              {
                // Call the start upload method on the first slice
                bytesUploaded = uploadFile.StartUpload(uploadId, strm);

                _logger.LogInformation($"UploadFileSlicePerSlice: uploading first slice...");
                _context.ExecuteQuery();

                // fileoffset is the pointer where the next slice will be added
                fileoffset = bytesUploaded.Value;
              }

              // NOTE:  small files have already been uploaded from lastBuffer, so reset it
              lastBuffer = new byte[0];
            }
          }

          // Get a reference to our file
          _logger.LogInformation($"UploadFileSlicePerSlice: getting reference to file...");
          uploadFile = _context.Web.GetFileByServerRelativeUrl(docClaimFolder.ServerRelativeUrl + Path.AltDirectorySeparatorChar + fileName);

          if (last)
          {
            // Is this the last slice of data?
            using (var strm = new MemoryStream(lastBuffer))
            {
              // End sliced upload by calling FinishUpload
              _logger.LogInformation($"UploadFileSlicePerSlice: uploading last slice...");
              uploadFile = uploadFile.FinishUpload(uploadId, fileoffset, strm);
              _context.Load(uploadFile);
              _context.ExecuteQuery();

              return uploadFile.UniqueId.ToString();
            }
          }

          if (first)
          {
            // we can only start the upload once
            first = false;

            continue;
          }

          using (var strm = new MemoryStream(buffer))
          {
            // Continue sliced upload
              _logger.LogInformation($"UploadFileSlicePerSlice: uploading intermediate slice...");
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
      return GetInternal(() =>
      {
        _logger.LogInformation($"EnumerateFolder: claimId: {claimId} | subFolder: {subFolder}");
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
          _logger.LogInformation($"EnumerateFolder: enumerating {_context.Url}/{claimFolderUrl}...");
          _context.ExecuteQuery();
        }
        catch (Exception ex)
        {
          _logger.LogInformation($"EnumerateFolder: {_context.Url}/{claimFolderUrl} does not exist");
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
              Length = 0,
              Url = new Uri(new Uri(_context.Url), x.ServerRelativeUrl).AbsoluteUri,
              TimeLastModified = x.TimeLastModified,
              BlobId = x.UniqueId.ToString()
            });
        var claimFileInfos = claimFolder
          .Files
          .Select(x =>
            new BlobInfo
            {
              Name = x.Name,
              IsFolder = false,
              Length = x.Length,
              Url = new Uri(new Uri(_context.Url), x.ServerRelativeUrl).AbsoluteUri,
              TimeLastModified = x.TimeLastModified,
              BlobId = x.UniqueId.ToString()
            });
        var retVal = new List<BlobInfo>();

        retVal.Add(claimFolderInfo);
        retVal.AddRange(claimSubFolderInfos);
        retVal.AddRange(claimFileInfos);

        return retVal;
      });
    }

    public FileStreamResult GetFileStream(IClaimsInfoProvider claimsInfoProvider, string claimId, string uniqueId)
    {
      return GetInternal(() =>
      {
        _logger.LogInformation($"GetFileStream: claimId: {claimId} | uniqueId: {uniqueId}");
        var file = _context.Web.GetFileById(Guid.Parse(uniqueId));
        _context.Load(file);
        _context.ExecuteQuery();
        _logger.LogInformation($"GetFileStream: retrieved info for {file.Name}");

        return
          new FileStreamResult(Microsoft.SharePoint.Client.NetCore.File.OpenBinaryDirect(_context, file.ServerRelativeUrl)?.Stream, GetContentType(file.Name))
          {
            FileDownloadName = Path.GetFileName(file.Name)
          };
      });
    }

    public void PrepareForSolution(IClaimsInfoProvider claimsInfoProvider, string solutionId)
    {
      GetInternal(() =>
      {
        _logger.LogInformation($"PrepareForSolution: solutionId: {solutionId}");
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

        return 0;
      });
    }

    private TOther GetInternal<TOther>(Func<TOther> get)
    {
      return _policy.Execute(get);
    }

    // can only create sub-folder immediately under baseUrl
    private void CreateSubFolder(string baseUrl, string subFolder)
    {
      _logger.LogInformation($"CreateSubFolder: baseUrl: {baseUrl} | subFolder: {subFolder}");
      var baseFolderExists = true;
      var baseFolder = _context.Web.GetFolderByServerRelativeUrl(Uri.EscapeUriString($"{baseUrl}"));
      _context.Load(baseFolder);
      try
      {
        _logger.LogInformation($"CreateSubFolder: checking base folder ({baseUrl}) exists...");
        _context.ExecuteQuery();
      }
      catch
      {
        _logger.LogInformation($"CreateSubFolder: base folder ({baseUrl}) does not exist");
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
        _logger.LogInformation($"CreateSubFolder: checking target folder ({baseUrl}/{subFolder}) exists...");
        _context.ExecuteQuery();
      }
      catch
      {
        _logger.LogInformation($"CreateSubFolder: target folder ({baseUrl}/{subFolder}) does not exist");
        targetFolderExists = false;
      }
      if (targetFolderExists)
      {
        return;
      }

      _logger.LogInformation($"CreateSubFolder: adding sub-folder ({subFolder}) ...");
      baseFolder.AddSubFolder(subFolder);
      _context.ExecuteQuery();
    }

    private static string GetContentType(string path)
    {
      var types = GetMimeTypes();
      var ext = Path.GetExtension(path).ToLowerInvariant();

      return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
    }

    private static Dictionary<string, string> GetMimeTypes()
    {
      return new Dictionary<string, string>
      {
          {".txt", "text/plain"},
          {".pdf", "application/pdf"},
          {".doc", "application/vnd.ms-word"},
          {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
          {".xls", "application/vnd.ms-excel"},
          {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
          {".ppt", "application/vnd.ms-powerpoint" },
          {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
          {".png", "image/png"},
          {".jpg", "image/jpeg"},
          {".jpeg", "image/jpeg"},
          {".gif", "image/gif"},
          {".csv", "text/csv"},
          {".mp4", "video/mp4"}
      };
    }
  }
}
