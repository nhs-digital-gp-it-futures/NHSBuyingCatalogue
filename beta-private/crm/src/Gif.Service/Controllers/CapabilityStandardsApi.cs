using Gif.Service.Attributes;
using Gif.Service.Const;
using Gif.Service.Crm;
using Gif.Service.Models;
using Gif.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace Gif.Service.Controllers
{
  /// <summary>
  /// capability standards controller
  /// </summary>
  [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + ",Bearer")]
  public class CapabilityStandardsApi : Controller
  {
    /// <summary>
    /// Retrieve all current capability standards in a paged list
    /// </summary>

    private readonly IConfiguration _config;

    public CapabilityStandardsApi(IConfiguration config)
    {
      _config = config;
    }

  /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
  /// <param name="pageSize">number of items per page.  Defaults to 20</param>
  /// <response code="200">Success - if no capability standards found, return empty list</response>
  [HttpGet]
    [Route("/api/CapabilityStandards")]
    [ValidateModelState]
    [SwaggerOperation("ApiCapabilityStandardsGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(PaginatedListCapabilityStandard), description: "Success - if no capability standards found, return empty list")]
    public virtual IActionResult ApiCapabilityStandardsGet([FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      IEnumerable<CapabilityStandard> capabilitiesStandard;
      int totalPages;

      try
      {
        var service = new CapabilityStandardService(new Repository(_config));
        capabilitiesStandard = service.GetAll();
        capabilitiesStandard = service.GetPagingValues(pageIndex, pageSize, capabilitiesStandard, out totalPages);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return new ObjectResult(new PaginatedListCapabilityStandard()
      {
        Items = capabilitiesStandard.ToList(),
        PageSize = pageSize ?? Paging.DefaultPageSize,
        TotalPages = totalPages,
        PageIndex = pageIndex ?? Paging.DefaultIndex
      });
    }

  }
}
