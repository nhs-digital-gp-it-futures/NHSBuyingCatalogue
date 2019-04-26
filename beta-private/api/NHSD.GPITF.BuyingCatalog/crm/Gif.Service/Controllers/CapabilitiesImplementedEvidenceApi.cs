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
  public class CapabilitiesImplementedEvidenceApiController : Controller
  {
    /// <summary>
    /// Get all EvidenceEntity for the given Claim  Each list is a distinct &#39;chain&#39; of EvidenceEntity ie original EvidenceEntity with all subsequent EvidenceEntity  The first item in each &#39;chain&#39; is the most current EvidenceEntity.  The last item in each &#39;chain&#39; is the original EvidenceEntity.
    /// </summary>

    private readonly ICapabilitiesImplementedEvidenceDatastore _datastore;

    public CapabilitiesImplementedEvidenceApiController(ICapabilitiesImplementedEvidenceDatastore datastore)
    {
      _datastore = datastore;
    }

  /// <param name="claimId">CRM identifier of Claim</param>
  /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
  /// <param name="pageSize">number of items per page.  Defaults to 20</param>
  /// <response code="200">Success</response>
  /// <response code="404">Claim not found</response>
  [HttpGet]
    [Route("/api/CapabilitiesImplementedEvidence/ByClaim/{claimId}")]
    [ValidateModelState]
    [SwaggerOperation("ApiCapabilitiesImplementedEvidenceByClaimByClaimIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(PaginatedListIEnumerableCapabilitiesImplementedEvidence), description: "Success")]
    public virtual IActionResult ApiCapabilitiesImplementedEvidenceByClaimByClaimIdGet([FromRoute][Required]string claimId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      IEnumerable<IEnumerable<CapabilityEvidence>> evidences;
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

      return new ObjectResult(new PaginatedListIEnumerableCapabilitiesImplementedEvidence()
      {
        Items = evidences.ToList(),
        TotalPages = totalPages,
        PageSize = pageSize ?? Paging.DefaultPageSize,
        PageIndex = pageIndex ?? Paging.DefaultIndex,
      });
    }

    /// <summary>
    /// Get an existing Capability Implemented for a given EvidenceEntity Id
    /// </summary>

    /// <param name="id">EvidenceEntity Id</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("/api/CapabilitiesImplementedEvidence/ById/{id}")]
    [ValidateModelState]
    [SwaggerOperation("ApiCapabilitiesImplementedEvidenceByIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(Solution), description: "Success")]
    public virtual IActionResult ApiCapabilitiesImplementedEvidenceByIdGet([FromRoute][Required]string id)
    {
      try
      {
        var capabilityImplemented = _datastore.ById(id);

        if (capabilityImplemented == null || capabilityImplemented?.Id == Guid.Empty)
          return StatusCode(404);

        return new ObjectResult(capabilityImplemented);

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
    [Route("/api/CapabilitiesImplementedEvidence")]
    [ValidateModelState]
    [SwaggerOperation("ApiCapabilitiesImplementedEvidencePost")]
    [SwaggerResponse(statusCode: 204, type: typeof(CapabilityEvidence), description: "Success")]
    public virtual IActionResult ApiCapabilitiesImplementedEvidencePost([FromBody]CapabilityEvidence evidenceEntity)
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