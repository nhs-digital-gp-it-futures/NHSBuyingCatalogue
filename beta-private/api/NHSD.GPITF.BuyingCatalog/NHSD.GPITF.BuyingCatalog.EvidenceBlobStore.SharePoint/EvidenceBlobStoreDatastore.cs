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

    public string AddEvidenceForClaim(IClaimsInfoProvider claimsInfoProvider, string claimId, Stream file, string filename, string subFolder = null)
    {
      var claim = claimsInfoProvider.GetClaimById(claimId);
      var soln = _solutionsDatastore.ById(claim.SolutionId);
      var org = _organisationsDatastore.ById(soln.OrganisationId);
      var subFolderseparator = !string.IsNullOrEmpty(subFolder) ? "/" : string.Empty;
      var claimFolder = $"{SharePoint_BaseUrl}/{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{claimsInfoProvider.GetFolderName()}/{claimsInfoProvider.GetFolderClaimName(claim)}";
      var absUri = new Uri($"{claimFolder}/{subFolder ?? string.Empty}{subFolderseparator}{filename}");
      var serverRelativeUrl = $"/{absUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)}";

      // create subFolder if not exists
      if (!string.IsNullOrEmpty(subFolder))
      {
        CreateSubFolder(claimFolder, subFolder);
      }

      //  Setting:
      //    overwriteIfExists =true
      // will create a new version of the file, if it already exists:
      //    https://sunbin0704.wordpress.com/2013/10/11/csom-update-file-and-create-new-version-in-document-library/
      Microsoft.SharePoint.Client.NetCore.File.SaveBinaryDirect(_context, serverRelativeUrl, file, true);

      return absUri.AbsoluteUri;
    }

    public IEnumerable<BlobInfo> EnumerateFolder(IClaimsInfoProvider claimsInfoProvider, string claimId, string subFolder = null)
    {
      var claim = claimsInfoProvider.GetClaimById(claimId);
      var soln = _solutionsDatastore.ById(claim.SolutionId);
      var org = _organisationsDatastore.ById(soln.OrganisationId);

      var claimFolderExists = true;
      var claimFolderUrl = $"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{claimsInfoProvider.GetFolderName()}/{claimsInfoProvider.GetFolderClaimName(claim)}/{subFolder ?? string.Empty}";
      var claimFolder = _context.Web.GetFolderByServerRelativeUrl(Uri.EscapeUriString(claimFolderUrl));

      _context.Load(claimFolder);
      _context.Load(claimFolder.Files);
      _context.Load(claimFolder.Folders);
      try
      {
        _context.ExecuteQuery();
      }
      catch
      {
        claimFolderExists = false;
      }
      if (!claimFolderExists)
      {
        throw new KeyNotFoundException($"Folder does not exist!: {_context.Url}/{claimFolderUrl}");
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
