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
  /// Create, find and retrieve AssessmentMessage for a Solution
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class ReviewsController : Controller
  {
    private readonly IReviewsLogic _logic;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="logic">business logic</param>
    public ReviewsController(IReviewsLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Retrieve all AssessmentMessage for a Solution in a paged list,
    /// given the Solution’s CRM identifier
    /// </summary>
    /// <param name="solutionId">CRM identifier of solution</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("BySolution/{solutionId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Reviews>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    public IActionResult BySolution([FromRoute][Required]string solutionId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var assMesses = _logic.BySolution(solutionId);
      var retval = PaginatedList<Reviews>.Create(assMesses, pageIndex, pageSize);
      return assMesses.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Create a new AssessmentMessage for a Solution
    /// </summary>
    /// <param name="assMess">new AssessmentMessage information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpPost]
    [Route("Create")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Reviews), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    [SwaggerRequestExample(typeof(Reviews), typeof(ReviewsExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Create([FromBody]Reviews assMess)
    {
      try
      {
        var newAssMess = _logic.Create(assMess);
        return new OkObjectResult(newAssMess);
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Update an existing AssessmentMessage with new information
    /// </summary>
    /// <param name="assMess">AssessmentMessage with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution or AssessmentMessage not found in CRM</response>
    [HttpPut]
    [Route("Update")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution or AssessmentMessage not found in CRM")]
    [SwaggerRequestExample(typeof(Reviews), typeof(ReviewsExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Update([FromBody]Reviews assMess)
    {
      try
      {
        _logic.Update(assMess);
        return new OkResult();
      }
      catch (Exception)
      {
        return StatusCode(418);
      }
    }

    /// <summary>
    /// Delete an existing AssessmentMessage for a Solution
    /// </summary>
    /// <param name="assMess">existing AssessmentMessage information</param>
    /// <response code="200">Success</response>
    /// <response code="404">AssessmentMessage not found in CRM</response>
    [HttpDelete]
    [Route("Delete")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "AssessmentMessage not found in CRM")]
    [SwaggerRequestExample(typeof(Reviews), typeof(ReviewsExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Delete([FromBody]Reviews assMess)
    {
      try
      {
        _logic.Delete(assMess);
        return new OkResult();
      }
      catch (Exception)
      {
        return StatusCode(418);
      }
    }
  }
}
