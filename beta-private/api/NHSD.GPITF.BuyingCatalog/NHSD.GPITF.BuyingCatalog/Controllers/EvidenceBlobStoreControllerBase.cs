using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.IO;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
#pragma warning disable CS1591
  public abstract class EvidenceBlobStoreControllerBase : Controller
  {
    protected readonly IEvidenceBlobStoreLogic _logic;

    public EvidenceBlobStoreControllerBase(IEvidenceBlobStoreLogic logic)
    {
      _logic = logic;
    }

    protected IActionResult AddEvidenceForClaimInternal(string claimId, IFormFile file, string filename, string subFolder = null)
    {
      try
      {
        var extUrl = _logic.AddEvidenceForClaim(claimId, file.OpenReadStream(), filename, subFolder);
        return new OkObjectResult(extUrl);
      }
      catch (FluentValidation.ValidationException ex)
      {
        return new InternalServerErrorObjectResult(ex);
      }
      catch (KeyNotFoundException ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    protected FileResult DownloadInternal(string claimId, string extUrl)
    {
      try
      {
        var stream = _logic.GetFileStream(claimId, extUrl);
        var memory = new MemoryStream();

        stream.CopyTo(memory);
        memory.Position = 0;

        var retval = File(memory, GetContentType(extUrl), Path.GetFileName(extUrl));

        return retval;
      }
      catch (FluentValidation.ValidationException)
      {
        return null;
      }
      catch (KeyNotFoundException)
      {
        return null;
      }
    }

    protected IActionResult EnumerateFolderInternal(string claimId, string subFolder, int? pageIndex, int? pageSize)
    {
      try
      {
        var infos = _logic.EnumerateFolder(claimId, subFolder);
        var retval = PaginatedList<BlobInfo>.Create(infos, pageIndex, pageSize);
        return new OkObjectResult(retval);
      }
      catch (FluentValidation.ValidationException ex)
      {
        return new InternalServerErrorObjectResult(ex);
      }
      catch (KeyNotFoundException ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    protected static string GetContentType(string path)
    {
      var types = GetMimeTypes();
      var ext = Path.GetExtension(path).ToLowerInvariant();

      return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
    }

    protected static Dictionary<string, string> GetMimeTypes()
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
#pragma warning restore CS1591
}
