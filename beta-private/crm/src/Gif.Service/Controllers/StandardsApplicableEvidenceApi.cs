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
  public class StandardsApplicableEvidenceApiController : Controller
  {
    /// <summary>
    /// Get all EvidenceEntity for the given Claim  Each list is a distinct &#39;chain&#39; of EvidenceEntity ie original EvidenceEntity with all subsequent EvidenceEntity  The first item in each &#39;chain&#39; is the most current EvidenceEntity.  The last item in each &#39;chain&#39; is the original EvidenceEntity.
    /// </summary>

    private readonly IStandardsApplicableEvidenceDatastore _datastore;

    public StandardsApplicableEvidenceApiController(IStandardsApplicableEvidenceDatastore datastore)
    {
      _datastore = datastore;
    }

  /// <param name="claimId">CRM identifier of Claim</param>
  /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
  /// <param name="pageSize">number of items per page.  Defaults to 20</param>
  /// <response code="200">Success</response>
  /// <response code="404">StandardsApplicable not found</response>
  [HttpGet]
    [Route("/api/StandardsApplicableEvidence/ByClaim/{claimId}")]
    [ValidateModelState]
    [SwaggerOperation("ApiStandardsApplicableEvidenceByClaimByClaimIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(PaginatedListIEnumerableStandardsApplicableEvidence), description: "Success")]
    public virtual IActionResult ApiStandardsApplicableEvidenceByClaimByClaimIdGet([FromRoute][Required]string claimId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      IEnumerable<IEnumerable<StandardApplicableEvidence>> evidences;
      int totalPages;

      try
      {
        evidences = _datastore.ByClaim(claimId);
        evidences = evidences.GetPagingValues(pageIndex, pageSize, out totalPages);

      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return new ObjectResult(new PaginatedListIEnumerableStandardsApplicableEvidence()
      {
        Items = evidences.ToList(),
        TotalPages = totalPages,
        PageSize = pageSize ?? Paging.DefaultPageSize,
        PageIndex = pageIndex ?? Paging.DefaultIndex,
      });
    }

    /// <summary>
    /// Get an existing Standard Applicable for a given EvidenceEntity Id
    /// </summary>

    /// <param name="id">EvidenceEntity Id</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("/api/StandardsApplicableEvidence/ById/{id}")]
    [ValidateModelState]
    [SwaggerOperation("ApiStandardsApplicableEvidenceByIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(Solution), description: "Success")]
    public virtual IActionResult ApiStandardsApplicableEvidenceByIdGet([FromRoute][Required]string id)
    {
      try
      {
        var standardApplicable = _datastore.ById(id);

        if (standardApplicable == null || standardApplicable?.Id == Guid.Empty)
          return StatusCode(404);

        return new ObjectResult(standardApplicable);

      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

    }

    /// <summary>
    /// Create a new evidenceEntity
    /// </summary>

    /// <param name="evidenceEntity">new evidenceEntity information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Claim not found</response>
    [HttpPost]
    [Route("/api/StandardsApplicableEvidence")]
    [ValidateModelState]
    [SwaggerOperation("ApiStandardsApplicableEvidencePost")]
    [SwaggerResponse(statusCode: 200, type: typeof(StandardApplicableEvidence), description: "Success")]
    public virtual IActionResult ApiStandardsApplicableEvidencePost([FromBody]StandardApplicableEvidence evidenceEntity)
    {
      try
      {
        evidenceEntity = _datastore.Create(evidenceEntity);

        if (evidenceEntity == null || evidenceEntity?.Id == Guid.Empty)
          return StatusCode(404);

        return new ObjectResult(evidenceEntity);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

    }
  }
}
