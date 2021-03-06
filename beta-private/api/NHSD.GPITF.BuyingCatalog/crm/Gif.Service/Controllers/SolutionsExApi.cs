using Gif.Service.Attributes;
using Gif.Service.Contracts;
using Gif.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace Gif.Service.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + ",Bearer")]
  public class SolutionsExApiController : Controller
  {
    /// <summary>
    /// Update an existing Solution, TechnicalContact, ClaimedCapability, ClaimedStandard et al with new information
    /// </summary>

    private readonly ISolutionsExDatastore _datastore;
    private readonly IFrameworksDatastore _frameworksDatastore;
    private readonly ILinkManagerDatastore _linkManagerDatastore;

    public SolutionsExApiController(
      ISolutionsExDatastore datastore,
      IFrameworksDatastore frameworksDatastore,
      ILinkManagerDatastore linkManagerDatastore)
    {
      _datastore = datastore;
      _frameworksDatastore = frameworksDatastore;
      _linkManagerDatastore = linkManagerDatastore;
    }


    /// <summary>
    /// Get a Solution with a list of corresponding TechnicalContact, ClaimedCapability, ClaimedStandard et al
    /// </summary>
    /// <param name="solutionId">CRM identifier of Solution</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("/api/porcelain/SolutionsEx/BySolution/{solutionId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(SolutionEx), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    [SwaggerOperation("ApiPorcelainSolutionsExBySolution")]
    public IActionResult BySolution([FromRoute][Required]string solutionId)
    {
      var solutionClaims = _datastore.BySolution(solutionId);
      return solutionClaims?.Solution != null ? (IActionResult)new OkObjectResult(solutionClaims) : new NotFoundResult();
    }


    /// <param name="solnEx">Solution, TechnicalContact, ClaimedCapability, ClaimedStandard et al with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution, TechnicalContact, ClaimedCapability, ClaimedStandard et al not found in CRM</response>
    /// <response code="500">Datastore exception</response>
    [HttpPut]
    [Route("/api/porcelain/SolutionsEx/Update")]
    [ValidateModelState]
    [SwaggerOperation("ApiPorcelainSolutionsExUpdatePut")]
    public virtual IActionResult ApiPorcelainSolutionsExUpdatePut([FromBody]SolutionEx solnEx)
    {
      var solutionFrameworks = new List<Framework>();

      try
      {
        solutionFrameworks = _frameworksDatastore.BySolution(solnEx.Solution.Id.ToString()).ToList();
        _datastore.Update(solnEx);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }
      finally
      {
        foreach (var solutionFramework in solutionFrameworks)
        {
          _linkManagerDatastore.FrameworkSolutionAssociate(solutionFramework.Id, solnEx.Solution.Id);
        }
      }

      return StatusCode(200, solnEx);
    }

    /// <summary>
    /// Get a list of Solutions, each with a list of corresponding TechnicalContact, ClaimedCapability, ClaimedStandard et al
    /// </summary>
    /// <param name="organisationId">CRM identifier of Organisation</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("/api/porcelain/SolutionsEx/ByOrganisation/{organisationId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<SolutionEx>), description: "Success")]
    [SwaggerOperation("ApiPorcelainSolutionsExByOrganisation")]
    public IActionResult ByOrganisation([FromRoute][Required]string organisationId)
    {
      var solnExs = _datastore.ByOrganisation(organisationId);
      return new OkObjectResult(solnExs);
    }
  }
}
