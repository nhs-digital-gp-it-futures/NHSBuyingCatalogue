using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Security;
using Microsoft.SharePoint.Client.NetCore.Runtime;
using Microsoft.SharePoint.Client.NetCore;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public class EvidenceBlobStoreLogic : IEvidenceBlobStoreLogic
  {
    protected const string CapabilityFolderName = "Capability Evidence";
    protected const string StandardsFolderName = "Standards Evidence";

    private readonly ClientContext _context;

    protected readonly IOrganisationsDatastore _organisationsDatastore;
    protected readonly ISolutionsDatastore _solutionsDatastore;
    protected readonly ICapabilitiesImplementedDatastore _capabilitiesImplementedDatastore;
    protected readonly IStandardsApplicableDatastore _standardsApplicableDatastore;
    protected readonly ICapabilitiesDatastore _capabilitiesDatastore;
    protected readonly IStandardsDatastore _standardsDatastore;

    private readonly string SharePoint_BaseUrl;
    private readonly string SharePoint_OrganisationsRelativeUrl;
    private readonly string SharePoint_Login;
    private readonly string SharePoint_Password;

    public EvidenceBlobStoreLogic(
      IConfiguration config,
      IOrganisationsDatastore organisationsDatastore,
      ISolutionsDatastore solutionsDatastore,
      ICapabilitiesImplementedDatastore capabilitiesImplementedDatastore,
      IStandardsApplicableDatastore standardsApplicableDatastore,
      ICapabilitiesDatastore capabilitiesDatastore,
      IStandardsDatastore standardsDatastore)
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

    public string AddEvidenceForClaim(string claimId, Stream file, string filename, string subFolder = null)
    {
      var claim = ClaimsDatastore.ById(claimId);
      if (claim == null)
      {
        throw new KeyNotFoundException($"Could not find claim: {claimId}");
      }
      var soln = _solutionsDatastore.ById(claim.SolutionId);
      var org = _organisationsDatastore.ById(soln.OrganisationId);
      var subFolderseparator = !string.IsNullOrEmpty(subFolder) ? "/" : string.Empty;
      var claimFolder = $"{SharePoint_BaseUrl}/{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{GetFolderName()}/{GetFolderClaimName(claim)}";
      var absUri = new Uri($"{claimFolder}/{subFolder ?? string.Empty}{subFolderseparator}{filename}");
      var serverRelativeUrl = $"/{absUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)}";

      // create subFolder if not exists
      if (!string.IsNullOrEmpty(subFolder))
      {
        CreateSubFolder(claimFolder, subFolder);
      }

      Microsoft.SharePoint.Client.NetCore.File.SaveBinaryDirect(_context, serverRelativeUrl, file, true);

      return absUri.AbsoluteUri;
    }

    public IEnumerable<BlobInfo> EnumerateFolder(string claimId, string subFolder = null)
    {
      var claim = ClaimsDatastore.ById(claimId);
      if (claim == null)
      {
        throw new KeyNotFoundException($"Could not find claim: {claimId}");
      }
      var soln = _solutionsDatastore.ById(claim.SolutionId);
      var org = _organisationsDatastore.ById(soln.OrganisationId);

      var claimFolderExists = true;
      var claimFolderUrl = $"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{GetFolderName()}/{GetFolderClaimName(claim)}/{subFolder ?? string.Empty}";
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

      retVal.AddRange(claimSubFolderInfos);
      retVal.AddRange(claimFileInfos);

      return retVal;
    }

    public void PrepareForSolution(string solutionId)
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
      CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}", CapabilityFolderName);
      CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}", StandardsFolderName);
      foreach (var folderName in claimedCapNames)
      {
        CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{CapabilityFolderName}", folderName);
      }
      foreach (var folderName in claimedNameStds)
      {
        CreateSubFolder($"{SharePoint_OrganisationsRelativeUrl}/{org.Name}/{soln.Name}/{StandardsFolderName}", folderName);
      }
    }

    protected virtual string GetFolderName()
    {
      throw new NotImplementedException();
    }

    protected virtual string GetFolderClaimName(ClaimsBase claim)
    {
      throw new NotImplementedException();
    }

    protected virtual IClaimsDatastore<ClaimsBase> ClaimsDatastore
    {
      get
      {
        throw new NotImplementedException();
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
