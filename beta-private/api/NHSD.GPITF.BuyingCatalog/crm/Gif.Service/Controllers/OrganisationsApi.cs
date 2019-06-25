using Gif.Service.Attributes;
using Gif.Service.Const;
using Gif.Service.Contracts;
using Gif.Service.Crm;
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
  public class OrganisationsApiController : Controller
  {
    private readonly IOrganisationsDatastore _datastore;

    public OrganisationsApiController(IOrganisationsDatastore datastore)
    {
      _datastore = datastore;
    }

    /// <summary>
    /// Retrieve an Organisation for the given Contact
    /// </summary>
    /// <param name="contactId">CRM identifier of Contact</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found</response>
    [HttpGet]
    [Route("/api/Organisations/ByContact/{contactId}")]
    [ValidateModelState]
    [SwaggerOperation("ApiOrganisationsByContactByContactIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(Organisation), description: "Success")]
    public virtual IActionResult ApiOrganisationsByContactByContactIdGet([FromRoute][Required]string contactId)
    {
      try
      {
        var organisation = _datastore.ByContact(contactId);

        if (organisation == null || organisation?.Id == Guid.Empty)
          return StatusCode(404);

        return new ObjectResult(organisation);
      }
      catch (CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }
    }

    /// <summary>
    /// Retrieve an Organisation for the given Id
    /// </summary>
    /// <param name="organisationId">CRM identifier of Organisation</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found</response>
    [HttpGet]
    [Route("/api/Organisations/ById/{organisationId}")]
    [ValidateModelState]
    [SwaggerOperation("ApiOrganisationsByOrganisationIdGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(Organisation), description: "Success")]
    public virtual IActionResult ApiOrganisationsByOrganisationIdGet([FromRoute][Required]string organisationId)
    {
      try
      {
        var organisation = _datastore.ById(organisationId);

        if (organisation == null || organisation?.Id == Guid.Empty)
          return StatusCode(404);

        return new ObjectResult(organisation);
      }
      catch (CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }
    }

    /// <summary>
    /// Retrieve all Organisations
    /// </summary>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success - if no organisations found, return empty list</response>
    [HttpGet]
    [Route("/api/Organisations")]
    [ValidateModelState]
    [SwaggerOperation("ApiOrganisationsGet")]
    [SwaggerResponse(statusCode: 200, type: typeof(PaginatedListOrganisations), description: "Success - if no organisations found, return empty list")]
    public virtual IActionResult ApiOrganisationsGet([FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      IEnumerable<Organisation> orgs;
      int totalPages;

      try
      {
        orgs = _datastore.GetAll();
        orgs = orgs.GetPagingValues(pageIndex, pageSize, out totalPages);
      }
      catch (Crm.CrmApiException ex)
      {
        return StatusCode((int)ex.HttpStatus, ex.Message);
      }

      return new ObjectResult(new PaginatedListOrganisations()
      {
        Items = orgs.ToList(),
        PageSize = pageSize ?? Paging.DefaultPageSize,
        TotalPages = totalPages,
        PageIndex = pageIndex ?? Paging.DefaultIndex
      });
    }
  }
}
