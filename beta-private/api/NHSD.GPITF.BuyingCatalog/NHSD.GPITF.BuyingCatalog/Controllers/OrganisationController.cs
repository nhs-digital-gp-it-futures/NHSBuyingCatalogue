using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
  /// <summary>
  /// Find and retrieve organisations
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class OrganisationController : Controller
  {
    private readonly IOrganisationLogic _logic;

    /// <summary>
    /// constructor for OrganisationController
    /// </summary>
    /// <param name="logic">business logic</param>
    public OrganisationController(IOrganisationLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Retrieve an organisation, given its ODS code.
    /// If the organisation does not currently exist:
    ///   * Retrieve ODS details from ODS API
    ///   * Create organisation/supplier from ODS details
    ///   * Return newly created organisation
    /// </summary>
    /// <param name="odsCode">ODS code of organisation to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found in ODS</response>
    [HttpGet]
    [Route("ByODS/{odsCode}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Organisation), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Organisation not found in ODS")]
    public IActionResult ByODS([FromRoute][Required]string odsCode)
    {
      var org = _logic.ByODS(odsCode);
      return org != null ? (IActionResult)new OkObjectResult(org) : new NotFoundResult();
    }

    /// <summary>
    /// Retrieve an organisation, given its unique identifier
    /// Unique identifier is case sensitive
    /// </summary>
    /// <param name="id">Unique identifier of organisation to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found in ODS</response>
    [HttpGet]
    [Route("ById/{id}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Organisation), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Organisation not found")]
    public IActionResult ById([FromRoute][Required]string id)
    {
      var org = _logic.ById(id);
      return org != null ? (IActionResult)new OkObjectResult(org) : new NotFoundResult();
    }

    /// <summary>
    /// Retrieve all organisations in a paged list
    /// </summary>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success - if no Organisation found, return empty list</response>
    [HttpGet]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Organisation>), description: "Success - if no Organisation found, return empty list")]
    public IActionResult Get([FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var allOrgs = _logic.GetAll();
      var retval = PaginatedList<Organisation>.Create(allOrgs, pageIndex, pageSize);
      return new OkObjectResult(retval);
    }

    /// <summary>
    /// Create a new Organisation [DEVELOPMENT ONLY]
    /// </summary>
    /// <param name="org">new Organisation information</param>
    /// <response code="200">Success</response>
    [HttpPost]
    [Route("Create")]
    [Authorize(
      Roles = Roles.Admin,
      AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Organisation), description: "Success")]
    public IActionResult Create([FromBody]Organisation org)
    {
      var newOrg = _logic.Create(org);
      return new OkObjectResult(newOrg);
    }

    /// <summary>
    /// Update information for an existing Organisation [DEVELOPMENT ONLY]
    /// </summary>
    /// <param name="org">Updated Organisation information</param>
    /// <response code="200">Success</response>
    [HttpPost]
    [Route("Update")]
    [Authorize(
      Roles = Roles.Admin,
      AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Organisation not found in CRM")]
    public IActionResult Update([FromBody]Organisation org)
    {
      try
      {
        _logic.Update(org);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Delete an existing Organisation [DEVELOPMENT ONLY]
    /// </summary>
    /// <param name="org">existing Organisation information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found in CRM</response>
    [HttpDelete]
    [Route("Delete")]
    [Authorize(
      Roles = Roles.Admin,
      AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Organisation not found in CRM")]
    public IActionResult Delete([FromBody]Organisation org)
    {
      try
      {
        _logic.Delete(org);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }
  }
}
