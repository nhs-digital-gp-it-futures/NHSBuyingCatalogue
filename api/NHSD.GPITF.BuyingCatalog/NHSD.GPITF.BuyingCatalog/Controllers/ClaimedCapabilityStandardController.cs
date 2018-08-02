using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Examples;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
  /// <summary>
  /// Manage ClaimedCapabilityStandard ie ClaimedStandard which support a ClaimedCapability
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class ClaimedCapabilityStandardController : Controller
  {
    private readonly IClaimedCapabilityStandardLogic _logic;

    /// <summary>
    /// constructor for ClaimedCapabilityStandardController
    /// </summary>
    /// <param name="logic">business logic</param>
    public ClaimedCapabilityStandardController(IClaimedCapabilityStandardLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Get ClaimedCapabilityStandard for the given ClaimedCapability
    /// </summary>
    /// <param name="claimedCapabilityId">CRM identifier of ClaimedCapability</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">ClaimedCapability not found in CRM</response>
    [HttpGet]
    [Route("ByClaimedCapability/{claimedCapabilityId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<ClaimedCapabilityStandard>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "ClaimedCapability not found in CRM")]
    public IActionResult ByFramework([FromRoute][Required]string claimedCapabilityId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var claimCapStd = _logic.ByClaimedCapability(claimedCapabilityId);
      var retval = PaginatedList<ClaimedCapabilityStandard>.Create(claimCapStd, pageIndex, pageSize);
      return claimCapStd.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Get ClaimedCapabilityStandard for the given Standard
    /// </summary>
    /// <param name="standardId">CRM identifier of ClaimedCapability</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Standard not found in CRM</response>
    [HttpGet]
    [Route("ByStandard/{standardId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<ClaimedCapabilityStandard>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Standard not found in CRM")]
    public IActionResult ByStandard([FromRoute][Required]string standardId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var claimCapStd = _logic.ByStandard(standardId);
      var retval = PaginatedList<ClaimedCapabilityStandard>.Create(claimCapStd, pageIndex, pageSize);
      return claimCapStd.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Create a new ClaimedCapabilityStandard for a ClaimedCapability
    /// </summary>
    /// <param name="claimedCapStd">new ClaimedCapabilityStandard information</param>
    /// <response code="200">Success</response>
    /// <response code="404">TODO not found in CRM</response>
    [HttpPost]
    [Route("Create")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(ClaimedCapabilityStandard), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    [SwaggerRequestExample(typeof(ClaimedCapabilityStandard), typeof(ClaimedCapabilityStandardExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Create([FromBody]ClaimedCapabilityStandard claimedCapStd)
    {
      try
      {
        var newStd = _logic.Create(claimedCapStd);
        return new OkObjectResult(newStd);
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Update an existing ClaimedCapabilityStandard with new information
    /// </summary>
    /// <param name="claimedCapStd">ClaimedCapabilityStandard with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">ClaimedCapabilityStandard not found in CRM</response>
    [HttpPut]
    [Route("Update")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "ClaimedCapabilityStandard not found in CRM")]
    [SwaggerRequestExample(typeof(ClaimedCapabilityStandard), typeof(ClaimedCapabilityStandardExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Update([FromBody]ClaimedCapabilityStandard claimedCapStd)
    {
      try
      {
        _logic.Update(claimedCapStd);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Delete an existing ClaimedCapabilityStandard for a ClaimedCapability
    /// </summary>
    /// <param name="claimedCapStd">existing ClaimedCapabilityStandard information</param>
    /// <response code="200">Success</response>
    /// <response code="404">ClaimedCapabilityStandard not found in CRM</response>
    [HttpDelete]
    [Route("Delete")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "ClaimedCapabilityStandard not found in CRM")]
    [SwaggerRequestExample(typeof(ClaimedCapabilityStandard), typeof(ClaimedCapabilityStandardExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Delete([FromBody]ClaimedCapabilityStandard claimedCapStd)
    {
      try
      {
        _logic.Delete(claimedCapStd);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }
  }
}
