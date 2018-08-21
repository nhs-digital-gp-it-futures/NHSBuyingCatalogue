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
  /// Manage frameworks
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class FrameworksController : Controller
  {
    private readonly IFrameworksLogic _logic;

    /// <summary>
    /// constructor for FrameworkController
    /// </summary>
    /// <param name="logic">business logic</param>
    public FrameworksController(IFrameworksLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Get existing framework/s which have the given capability
    /// </summary>
    /// <param name="capabilityId">CRM identifier of Capability</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Capability not found in CRM</response>
    [HttpGet]
    [Route("ByCapability/{capabilityId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Frameworks>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Capability not found in CRM")]
    public IActionResult ByCapability([FromRoute][Required]string capabilityId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var frameworks = _logic.ByCapability(capabilityId);
      var retval = PaginatedList<Frameworks>.Create(frameworks, pageIndex, pageSize);
      return frameworks.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Get existing framework/s which have the given standard
    /// </summary>
    /// <param name="standardId">CRM identifier of Standard</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Standard not found in CRM</response>
    [HttpGet]
    [Route("ByStandard/{standardId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Frameworks>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Standard not found in CRM")]
    public IActionResult ByStandard([FromRoute][Required]string standardId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var frameworks = _logic.ByStandard(standardId);
      var retval = PaginatedList<Frameworks>.Create(frameworks, pageIndex, pageSize);
      return frameworks.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Get an existing framework given its CRM identifier
    /// Typically used to retrieve previous version
    /// </summary>
    /// <param name="id">CRM identifier of framework to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Framework not found in CRM</response>
    [HttpGet]
    [Route("ById/{id}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Frameworks), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Framework not found in CRM")]
    public IActionResult ById([FromRoute][Required]string id)
    {
      var framework = _logic.ById(id);
      return framework != null ? (IActionResult)new OkObjectResult(framework) : new NotFoundResult();
    }

    /// <summary>
    /// Get existing framework/s on which a solution was onboarded, given the CRM identifier of the solution
    /// </summary>
    /// <param name="solutionId">CRM identifier of solution</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("BySolution/{solutionId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Frameworks>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    public IActionResult BySolution([FromRoute][Required]string solutionId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var frameworks = _logic.BySolution(solutionId);
      var retval = PaginatedList<Frameworks>.Create(frameworks, pageIndex, pageSize);
      return frameworks.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Create a new framework
    /// </summary>
    /// <param name="framework">new framework information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Previous framework not found in CRM</response>
    [HttpPost]
    [Route("Create")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Frameworks), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Previous framework not found in CRM")]
    [SwaggerRequestExample(typeof(Frameworks), typeof(FrameworksExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Create([FromBody]Frameworks framework)
    {
      var newFramework = _logic.Create(framework);
      return newFramework != null ? (IActionResult)new OkObjectResult(newFramework) : new NotFoundResult();
    }

    /// <summary>
    /// Retrieve all current frameworks in a paged list
    /// </summary>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success - if no frameworks found, return empty list</response>
    [HttpGet]
    [ValidateModelState]
    [SwaggerResponse(statusCode: 200, type: typeof(PaginatedList<Frameworks>), description: "Success - if no frameworks found, return empty list")]
    public IActionResult Get([FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var allFrameworks = _logic.GetAll();
      var retval = PaginatedList<Frameworks>.Create(allFrameworks, pageIndex, pageSize);
      return new OkObjectResult(retval);
    }

    /// <summary>
    /// Update an existing framework with new information
    /// </summary>
    /// <param name="framework">framework with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Framework or previous version not found in CRM</response>
    [HttpPut]
    [Route("Update")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Framework or previous version not found in CRM")]
    [SwaggerRequestExample(typeof(Frameworks), typeof(FrameworksExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Update([FromBody]Frameworks framework)
    {
      try
      {
        _logic.Update(framework);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }
  }
}
