using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
  /// Create, find and retrieve AssessmentMessageContact for a AssessmentMessage
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class ReviewContactController : Controller
  {
    private readonly IReviewContactLogic _logic;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="logic">business logic</param>
    public ReviewContactController(IReviewContactLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Retrieve all AssessmentMessageContacts for a AssessmentMessage in a paged list,
    /// given the Solution’s CRM identifier
    /// </summary>
    /// <param name="assMessId">CRM identifier of AssessmentMessage</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("ByAssessmentMessage/{assMessId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<ReviewContact>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    public IActionResult ByAssessmentMessage([FromRoute][Required]string assMessId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var assMessConts = _logic.ByAssessmentMessage(assMessId);
      var retval = PaginatedList<ReviewContact>.Create(assMessConts, pageIndex, pageSize);
      return assMessConts.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Create a new AssessmentMessageContact for an AssessmentMessage
    /// </summary>
    /// <param name="assMessCont">new AssessmentMessageContact information</param>
    /// <response code="200">Success</response>
    /// <response code="404">AssessmentMessage or Contact not found in CRM</response>
    [HttpPost]
    [Route("Create")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(ReviewContact), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "AssessmentMessage or Contact not found in CRM")]
    [SwaggerRequestExample(typeof(ReviewContact), typeof(ReviewContactExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Create([FromBody]ReviewContact assMessCont)
    {
      try
      {
        var newAssMessCont = _logic.Create(assMessCont);
        return new OkObjectResult(newAssMessCont);
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Update an existing AssessmentMessageContact with new information
    /// </summary>
    /// <param name="assMessCont">AssessmentMessageContact with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">AssessmentMessageContact not found in CRM</response>
    [HttpPut]
    [Route("Update")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "AssessmentMessageContact not found in CRM")]
    [SwaggerRequestExample(typeof(ReviewContact), typeof(ReviewContactExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Update([FromBody]ReviewContact assMessCont)
    {
      try
      {
        _logic.Update(assMessCont);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Delete an existing AssessmentMessageContact for an AssessmentMessage
    /// </summary>
    /// <param name="assMessCont">existing AssessmentMessageContact information</param>
    /// <response code="200">Success</response>
    /// <response code="404">AssessmentMessageContact not found in CRM</response>
    [HttpDelete]
    [Route("Delete")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "AssessmentMessageContact not found in CRM")]
    [SwaggerRequestExample(typeof(ReviewContact), typeof(ReviewContactExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Delete([FromBody]ReviewContact assMessCont)
    {
      try
      {
        _logic.Delete(assMessCont);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }
  }
}
