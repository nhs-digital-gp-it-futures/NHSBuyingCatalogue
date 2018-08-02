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
  /// Manage claimed capabilities
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class ClaimedCapabilityController : Controller
  {
    private readonly IClaimedCapabilityLogic _logic;

    /// <summary>
    /// constructor for ClaimedCapabilityController
    /// </summary>
    /// <param name="logic">business logic</param>
    public ClaimedCapabilityController(IClaimedCapabilityLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Retrieve all claimed capabilities for a solution in a paged list,
    /// given the solution’s CRM identifier
    /// </summary>
    /// <param name="solutionId">CRM identifier of solution</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("BySolution/{solutionId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<ClaimedCapability>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    public IActionResult BySolution([FromRoute][Required]string solutionId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var caps = _logic.BySolution(solutionId);
      var retval = PaginatedList<ClaimedCapability>.Create(caps, pageIndex, pageSize);
      return caps.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Create a new claimed capability for a solution
    /// </summary>
    /// <param name="claimedcapability">new claimed capability information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpPost]
    [Route("Create")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(ClaimedCapability), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    [SwaggerRequestExample(typeof(ClaimedCapability), typeof(ClaimedCapabilityExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Create([FromBody]ClaimedCapability claimedcapability)
    {
      try
      {
        var newStd = _logic.Create(claimedcapability);
        return new OkObjectResult(newStd);
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Update an existing claimed capability with new information
    /// </summary>
    /// <param name="claimedcapability">claimed capability with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution or ClaimedCapability not found in CRM</response>
    [HttpPut]
    [Route("Update")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution or ClaimedCapability not found in CRM")]
    [SwaggerRequestExample(typeof(ClaimedCapability), typeof(ClaimedCapabilityExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Update([FromBody]ClaimedCapability claimedcapability)
    {
      try
      {
        _logic.Update(claimedcapability);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Delete an existing claimed capability for a solution
    /// </summary>
    /// <param name="claimedcapability">existing claimed capability information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Claimed standard not found in CRM</response>
    [HttpDelete]
    [Route("Delete")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "ClaimedCapability not found in CRM")]
    [SwaggerRequestExample(typeof(ClaimedCapability), typeof(ClaimedCapabilityExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Delete([FromBody]ClaimedCapability claimedcapability)
    {
      try
      {
        _logic.Delete(claimedcapability);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }
  }
}
