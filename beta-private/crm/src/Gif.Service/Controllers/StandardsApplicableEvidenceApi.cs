/*
 * catalogue-api
 *
 * NHS Digital GP IT Futures Buying Catalog API
 *
 * OpenAPI spec version: 1.0.0-private-beta
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using Gif.Service.Attributes;
using Gif.Service.Const;
using Gif.Service.Crm;
using Gif.Service.Models;
using Gif.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gif.Service.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class StandardsApplicableEvidenceApiController : Controller
    {
        /// <summary>
        /// Get all Evidence for the given Claim  Each list is a distinct &#39;chain&#39; of Evidence ie original Evidence with all subsequent Evidence  The first item in each &#39;chain&#39; is the most current Evidence.  The last item in each &#39;chain&#39; is the original Evidence.
        /// </summary>

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
            IEnumerable<Evidence> evidences;
            int totalPages;

            try
            {
                var service = new StandardsApplicableEvidenceService(new Repository());
                evidences = service.ByClaim(claimId);
                evidences = service.GetPagingValues(pageIndex, pageSize, evidences, out totalPages);

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
        /// Get an existing Standard Applicable for a given Evidence Id
        /// </summary>

        /// <param name="id">Evidence Id</param>
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
                var standardApplicable = new StandardsApplicableEvidenceService(new Repository()).ByEvidenceId(id);

                if (standardApplicable.Id == Guid.Empty)
                    return StatusCode(404);

                return new ObjectResult(standardApplicable);

            }
            catch (Crm.CrmApiException ex)
            {
                return StatusCode((int)ex.HttpStatus, ex.Message);
            }

        }

        /// <summary>
        /// Get an existing Standard Applicable for a given Review Id
        /// </summary>

        /// <param name="id">Review Id</param>
        /// <response code="200">Success</response>
        /// <response code="404">Solution not found in CRM</response>
        [HttpGet]
        [Route("/api/StandardsApplicableReview/ById/{id}")]
        [ValidateModelState]
        [SwaggerOperation("StandardsApplicableReviewByIdGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(Solution), description: "Success")]
        public virtual IActionResult StandardsApplicableReviewByIdGet([FromRoute][Required]string id)
        {
            try
            {
                var standardApplicable = new StandardsApplicableEvidenceService(new Repository()).ByReviewId(id);

                if (standardApplicable.Id == Guid.Empty)
                    return StatusCode(404);

                return new ObjectResult(standardApplicable);

            }
            catch (Crm.CrmApiException ex)
            {
                return StatusCode((int)ex.HttpStatus, ex.Message);
            }

        }

        /// <summary>
        /// Create a new evidence
        /// </summary>

        /// <param name="evidence">new evidence information</param>
        /// <response code="200">Success</response>
        /// <response code="404">Claim not found</response>
        [HttpPost]
        [Route("/api/StandardsApplicableEvidence")]
        [ValidateModelState]
        [SwaggerOperation("ApiStandardsApplicableEvidencePost")]
        [SwaggerResponse(statusCode: 200, type: typeof(Evidence), description: "Success")]
        public virtual IActionResult ApiStandardsApplicableEvidencePost([FromBody]Evidence evidence)
        {
            try
            {
                evidence = new StandardsApplicableEvidenceService(new Repository()).Create(evidence);
            }
            catch (Crm.CrmApiException ex)
            {
                return StatusCode((int)ex.HttpStatus, ex.Message);
            }

            return StatusCode(204);
        }
    }
}
