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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
  /// <summary>
  /// Manage standards
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class StandardController : Controller
  {
    private readonly IStandardLogic _logic;

    /// <summary>
    /// constructor for StandardController
    /// </summary>
    /// <param name="logic">business logic</param>
    public StandardController(IStandardLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Get existing/optional Standard/s which are in the given Capability
    /// </summary>
    /// <param name="capabilityId">CRM identifier of Capability</param>
    /// <param name="isOptional">true if the specified Standard is optional with the Capability</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Capability not found in CRM</response>
    [HttpGet]
    [Route("ByCapability/{capabilityId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Standard>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Capability not found in CRM")]
    public IActionResult ByCapability([FromRoute][Required]string capabilityId, [FromQuery][Required]bool isOptional, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var stds = _logic.ByCapability(capabilityId, isOptional);
      var retval = PaginatedList<Standard>.Create(stds, pageIndex, pageSize);
      return stds.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Get existing Standard/s which are in the given Framework
    /// </summary>
    /// <param name="frameworkId">CRM identifier of Framework</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Framework not found in CRM</response>
    [HttpGet]
    [Route("ByFramework/{frameworkId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Standard>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Framework not found in CRM")]
    public IActionResult ByFramework([FromRoute][Required]string frameworkId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var stds = _logic.ByFramework(frameworkId);
      var retval = PaginatedList<Standard>.Create(stds, pageIndex, pageSize);
      return stds.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Get an existing standard given its CRM identifier
    /// Typically used to retrieve previous version
    /// </summary>
    /// <param name="id">CRM identifier of standard to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Standard not found in CRM</response>
    [HttpGet]
    [Route("ById/{id}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Standard), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Standard not found in CRM")]
    public IActionResult ById([FromRoute][Required]string id)
    {
      var std = _logic.ById(id);
      return std != null ? (IActionResult)new OkObjectResult(std) : new NotFoundResult();
    }

    /// <summary>
    /// Get several existing Standards given their CRM identifiers
    /// </summary>
    /// <param name="ids">Array of CRM identifiers of Standards to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Standards not found in CRM</response>
    [HttpPost]
    [Route("ByIds")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<Standard>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Standards not found in CRM")]
    [SwaggerRequestExample(typeof(IEnumerable<string>), typeof(StandardIdsExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult ByIds([FromBody][Required]IEnumerable<string> ids)
    {
      var stds = _logic.ByIds(ids);
      return stds.Count() > 0 ? (IActionResult)new OkObjectResult(stds) : new NotFoundResult();
    }

    /// <summary>
    /// Create a new standard
    /// </summary>
    /// <param name="standard">new standard information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Previous standard not found in CRM</response>
    [HttpPost]
    [Route("Create")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Standard), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Previous standard not found in CRM")]
    [SwaggerRequestExample(typeof(Standard), typeof(StandardExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Create([FromBody]Standard standard)
    {
      var newStd = _logic.Create(standard);
      return newStd != null ? (IActionResult)new OkObjectResult(newStd) : new NotFoundResult();
    }

    /// <summary>
    /// Retrieve all current standards in a paged list
    /// </summary>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success - if no standards found, return empty list</response>
    [HttpGet]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Standard>), description: "Success - if no standards found, return empty list")]
    public IActionResult Get([FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var allStds = _logic.GetAll();
      var retval = PaginatedList<Standard>.Create(allStds, pageIndex, pageSize);
      return new OkObjectResult(retval);
    }

    /// <summary>
    /// Update an existing standard with new information
    /// </summary>
    /// <param name="standard">standard with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Standard or previous version not found in CRM</response>
    [HttpPut]
    [Route("Update")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Standard or previous version not found in CRM")]
    [SwaggerRequestExample(typeof(Standard), typeof(StandardExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Update([FromBody]Standard standard)
    {
      try
      {
        _logic.Update(standard);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }
  }
}
