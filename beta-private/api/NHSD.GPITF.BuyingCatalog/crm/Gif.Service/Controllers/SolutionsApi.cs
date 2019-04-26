using Gif.Service.Attributes;
using Gif.Service.Const;
using Gif.Service.Contracts;
using Gif.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace Gif.Service.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + ",Bearer")]
  public class SolutionsApiController : Controller
  {
    /// <summary>
    /// Get existing solution/s on which were onboarded onto a framework,  given the CRM identifier of the framework
    /// </summary>

    private readonly ISolutionsDatastore _datastore;

    public SolutionsApiController(ISolutionsDatastore datastore)
    {
      _datastore = datastore;
    }

  /// <param name="frameworkId">CRM identifier of organisation to find</param>
  /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
  /// <param name="pageSize">number of items per page.  Defaults to 20</param>
  /// <response code="200">Success</response>
  /// <response code="404">Framework not found in CRM</response>
  [HttpGet]
    [Route("/api/Solutions/ByFramework/{frameworkId}")]
    [ValidateModelState]
    [SwaggerOperation("ApiSolutionsByFrameworkByFrameworkIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(PaginatedListSolutions), description: "Success")]
    public virtual IActionResult ApiSolutionsByFrameworkByFrameworkIdGet([FromRoute][Required]string frameworkId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {

      IEnumerable<Solution> solutions;
      int totalPages;

      try
      {
        solutions = _datastore.ByFramework(frameworkId);
        solutions = solutions.GetPagingValues(pageIndex, pageSize, out totalPages);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return new ObjectResult(new PaginatedListSolutions()
      {
        Items = solutions.ToList(),
        TotalPages = totalPages,
        PageIndex = pageIndex ?? Paging.DefaultIndex,
        PageSize = pageSize ?? Paging.DefaultPageSize
      });

    }

    /// <summary>
    /// Get an existing solution given its CRM identifier  Typically used to retrieve previous version
    /// </summary>

    /// <param name="id">CRM identifier of solution to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("/api/Solutions/ById/{id}")]
    [ValidateModelState]
    [SwaggerOperation("ApiSolutionsByIdByIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(Solution), description: "Success")]
    public virtual IActionResult ApiSolutionsByIdByIdGet([FromRoute][Required]string id)
    {
      try
      {
        var solution = _datastore.ById(id);

        if (solution == null || solution?.Id == Guid.Empty)
          return StatusCode(404);

        return new ObjectResult(solution);

      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

    }

    /// <summary>
    /// Retrieve all current solutions in a paged list for an organisation,  given the organisation’s CRM identifier
    /// </summary>

    /// <param name="organisationId">CRM identifier of organisation</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found in CRM</response>
    [HttpGet]
    [Route("/api/Solutions/ByOrganisation/{organisationId}")]
    [ValidateModelState]
    [SwaggerOperation("ApiSolutionsByOrganisationByOrganisationIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(PaginatedListSolutions), description: "Success")]
    public virtual IActionResult ApiSolutionsByOrganisationByOrganisationIdGet([FromRoute][Required]string organisationId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      IEnumerable<Solution> solutions;
      int totalPages;

      try
      {
        solutions = _datastore.ByOrganisation(organisationId);
        solutions = solutions.GetPagingValues(pageIndex, pageSize, out totalPages);

      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return new ObjectResult(new PaginatedListSolutions()
      {
        Items = solutions.ToList(),
        TotalPages = totalPages,
        PageIndex = pageIndex ?? Paging.DefaultIndex,
        PageSize = pageSize ?? Paging.DefaultPageSize,
      });
    }

    /// <summary>
    /// Create a new solution for an organisation
    /// </summary>

    /// <param name="solution">new solution information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found in CRM</response>
    /// <response code="500">Validation exception</response>
    [HttpPost]
    [Route("/api/Solutions")]
    [ValidateModelState]
    [SwaggerOperation("ApiSolutionsPost")]
    [SwaggerResponse(statusCode: 200, type: typeof(Solution), description: "Success")]
    public virtual IActionResult ApiSolutionsPost([FromBody]Solution solution)
    {
      try
      {
        solution = _datastore.Create(solution);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return new ObjectResult(solution);
    }

    /// <summary>
    /// Update an existing solution with new information
    /// </summary>

    /// <param name="solution">contact with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation or solution not found in CRM</response>
    [HttpPut]
    [Route("/api/Solutions")]
    [ValidateModelState]
    [SwaggerOperation("ApiSolutionsPut")]
    public virtual IActionResult ApiSolutionsPut([FromBody]Solution solution)
    {
      try
      {
        _datastore.Update(solution);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return StatusCode(200, solution);
    }

    /// <summary>
    /// Delete an existing solution
    /// </summary>

    /// <param name="Solution">existing solution object</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpDelete]
    [Route("/api/Solutions")]
    [ValidateModelState]
    [SwaggerOperation("ApiSolutionsDelete")]
    public virtual IActionResult ApiStandardsApplicableDelete([FromBody][Required]Solution solution)
    {
      try
      {
        var solutionGet = _datastore.ById(solution.Id.ToString());

        if (solutionGet == null || solutionGet?.Id == Guid.Empty)
          return StatusCode(404);

        _datastore.Delete(solution);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return StatusCode(200);
    }
  }
}