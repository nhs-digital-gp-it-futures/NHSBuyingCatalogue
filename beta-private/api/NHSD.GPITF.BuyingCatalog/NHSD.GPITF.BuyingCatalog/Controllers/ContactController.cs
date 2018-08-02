using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
  /// Create, find and retrieve contacts
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class ContactController : Controller
  {
    private readonly IContactLogic _logic;

    /// <summary>
    /// constructor for ContactController
    /// </summary>
    /// <param name="logic">business logic</param>
    public ContactController(IContactLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Retrieve all contacts for an organisation in a paged list, given the organisation’s CRM identifier
    /// </summary>
    /// <param name="organisationId">CRM identifier of organisation</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found in ODS API</response>
    [HttpGet]
    [Route("ByOrganisation/{organisationId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Contact>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Organisation not found in ODS")]
    public IActionResult ByOrganisation([FromRoute][Required]string organisationId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var contacts = _logic.ByOrganisation(organisationId);
      var retval = PaginatedList<Contact>.Create(contacts, pageIndex, pageSize);
      return contacts.Count() > 0 ? (IActionResult)new OkObjectResult(retval) : new NotFoundResult();
    }

    /// <summary>
    /// Retrieve a contacts for an organisation, given the contact’s email address
    /// Email address is case insensitive
    /// </summary>
    /// <param name="email">email address to search for</param>
    /// <response code="200">Success</response>
    /// <response code="404">Contact not found</response>
    [HttpGet]
    [Route("ByEmail/{email}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Contact), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Contact not found")]
    public IActionResult ByEmail([FromRoute][Required]string email)
    {
      var contact = _logic.ByEmail(email);
      return contact != null ? (IActionResult)new OkObjectResult(contact) : new NotFoundResult();
    }

    /// <summary>
    /// Retrieve a contact for an organisation, given the contact’s CRM identifier
    /// </summary>
    /// <param name="id">CRM identifier of contact</param>
    /// <response code="200">Success</response>
    /// <response code="404">Contact not found</response>
    [HttpGet]
    [Route("ById/{id}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Contact), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Contact not found")]
    public IActionResult ById([FromRoute][Required]string id)
    {
      var contact = _logic.ById(id);
      return contact != null ? (IActionResult)new OkObjectResult(contact) : new NotFoundResult();
    }

    /// <summary>
    /// Create a new Contact for an Organisation [DEVELOPMENT ONLY]
    /// </summary>
    /// <param name="contact">new contact information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation not found in CRM</response>
    [HttpPost]
    [Route("Create")]
    [Authorize(
      Roles = Roles.Admin,
      AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Contact), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Organisation not found in ODS")]
    [SwaggerRequestExample(typeof(Contact), typeof(ContactExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Create([FromBody]Contact contact)
    {
      try
      {
        var newContact = _logic.Create(contact);
        return newContact != null ? (IActionResult)new OkObjectResult(newContact) : new NotFoundResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Update an existing Contact with new information [DEVELOPMENT ONLY]
    /// </summary>
    /// <param name="contact">contact with updated information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation or Contact not found in CRM</response>
    [HttpPut]
    [Route("Update")]
    [Authorize(
      Roles = Roles.Admin,
      AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Organisation not found in ODS")]
    [SwaggerRequestExample(typeof(Contact), typeof(ContactExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Update([FromBody]Contact contact)
    {
      try
      {
        _logic.Update(contact);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }

    /// <summary>
    /// Delete an existing Contact [DEVELOPMENT ONLY]
    /// </summary>
    /// <param name="contact">existing Organisation information</param>
    /// <response code="200">Success</response>
    /// <response code="404">Organisation or Contact not found in CRM</response>
    [HttpDelete]
    [Route("Delete")]
    [Authorize(
      Roles = Roles.Admin,
      AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Organisation not found in CRM")]
    [SwaggerRequestExample(typeof(Contact), typeof(ContactExample), jsonConverter: typeof(StringEnumConverter))]
    public IActionResult Delete([FromBody]Contact contact)
    {
      try
      {
        _logic.Delete(contact);
        return new OkResult();
      }
      catch (Exception ex)
      {
        return new NotFoundObjectResult(ex);
      }
    }
  }
}
