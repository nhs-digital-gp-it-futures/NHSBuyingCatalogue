using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.OperationFilters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
  /// <summary>
  /// Manage capabilities evidence
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class CapabilitiesImplementedEvidenceBlobStoreController : Controller
  {
    private readonly ICapabilitiesImplementedEvidenceBlobStoreLogic _logic;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="logic">business logic</param>
    public CapabilitiesImplementedEvidenceBlobStoreController(ICapabilitiesImplementedEvidenceBlobStoreLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Upload a file to support a claim
    /// </summary>
    /// <param name="claimId">unique identifier of solution claim</param>
    /// <param name="file">Stream representing file to be uploaded</param>
    /// <param name="filename">name of file on the server</param>
    /// <param name="subFolder">optional sub-folder under claim.  This will be created if it does not exist.</param>
    /// <remarks>
    /// Server side folder structure is something like:
    /// --Organisation
    /// ----Solution
    /// ------Capability Evidence
    /// --------Appointment Management - Citizen
    /// --------Appointment Management - GP
    /// --------Clinical Decision Support
    /// --------[all other claimed capabilities]
    /// ----------Images
    /// ----------PDF
    /// ----------Videos
    /// ----------RTM
    /// ----------Misc
    ///
    /// where subFolder is an optional folder under a claimed capability ie Images, PDF, et al
    /// </remarks>
    /// <returns>externally accessible URL of file</returns>
    [HttpPost]
    [Route("AddEvidenceForClaim")]
    [SwaggerOperationFilter(typeof(EvidenceForClaimFileUploadOperation))]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(string), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Claim not found in CRM")]
    public IActionResult AddEvidenceForClaim([Required]string claimId, [Required]IFormFile file, [Required]string filename, string subFolder = null)
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

    /// <summary>
    /// List all files and sub-folders for a claim including folder for claim
    /// </summary>
    /// <param name="claimId">unique identifier of solution claim</param>
    /// <param name="subFolder">optional sub-folder under claim</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <returns>list of BlobInfo - first item is folder for claim</returns>
    [HttpGet]
    [Route("EnumerateFolder/{claimId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<BlobInfo>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Claim not found in CRM")]
    public IActionResult EnumerateFolder([FromRoute][Required]string claimId, [FromQuery]string subFolder, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
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
  }
}
