#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using Gif.Service.Attributes;
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gif.Service.Services
{
    public class OrganisationsService : ServiceBase, IOrganisationsDatastore
    {
        public OrganisationsService(IRepository repository) : base(repository)
        {
        }

        public Organisation ByContact(string contactId)
        {
            var contactService = new ContactsService(Repository);
            var contact = contactService.ById(contactId);

            return contact?.Organisation == Guid.Empty ? null :
                ById(contact?.Organisation.ToString());
        }

        public Organisation ById(string organisationId)
        {
            var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("OrganisationId") {FilterName = "accountid", FilterValue = organisationId},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

            var appJson = Repository.RetrieveMultiple(new Organisation().GetQueryString(null, filterAttributes), out Count);
            var organisation = appJson?.FirstOrDefault();

            return new Organisation(organisation);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member