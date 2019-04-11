using Gif.Service.Attributes;
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Gif.Service.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  public class LinkManagerApiController : Controller
  {
    /// <summary>
    /// Create a link between a Framework and a Solution
    /// </summary>

    private readonly ILinkManagerDatastore _datastore;

    public LinkManagerApiController(ILinkManagerDatastore datastore)
    {
      _datastore = datastore;
    }

  /// <param name="frameworkId">CRM identifier of Framework</param>
  /// <param name="solutionId">CRM identifier of Solution</param>
  /// <response code="200">Success</response>
  /// <response code="404">One entity not found</response>
  /// <response code="412">Link already exists</response>
  [HttpPost]
    [Route("/api/LinkManager/FrameworkSolution/Create/{frameworkId}/{solutionId}")]
    [ValidateModelState]
    [SwaggerOperation("ApiLinkManagerFrameworkSolutionCreateByFrameworkIdBySolutionIdPost")]
    public virtual IActionResult ApiLinkManagerFrameworkSolutionCreateByFrameworkIdBySolutionIdPost([FromRoute][Required]string frameworkId, [FromRoute][Required]string solutionId)
    {
      try
      {
        Guid frameworkIdParsed, solutionIdParsed;
        Guid.TryParse(frameworkId, out frameworkIdParsed);
        Guid.TryParse(solutionId, out solutionIdParsed);

        if (solutionIdParsed == Guid.Empty || frameworkIdParsed == Guid.Empty)
          throw new CrmApiException("Cannot parse strings into Guids", HttpStatusCode.BadRequest);

        _datastore.FrameworkSolutionAssociate(frameworkIdParsed, solutionIdParsed);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return new ObjectResult(200);
    }
  }
}
