using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  internal static class Retriever
  {
    public static List<Contacts> GetAllContacts(ISyncPolicyFactory _policy)
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll().ToList();
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var allSolns = frameworks.SelectMany(fw => solnDatastore.ByFramework(fw.Id)).ToList();
      var allOrgIds = allSolns.Select(soln => soln.OrganisationId).Distinct().ToList();
      var datastore = new ContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<ContactsDatastore>>().Object, _policy);
      var datas = allOrgIds.SelectMany(orgId => datastore.ByOrganisation(orgId)).ToList();

      return datas;
    }

    public static List<Solutions> GetAllSolutions(ISyncPolicyFactory _policy)
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll().ToList();
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var allSolns = frameworks.SelectMany(fw => solnDatastore.ByFramework(fw.Id)).ToList();

      return allSolns;
    }

    public static List<Organisations> GetAllOrganisations(ISyncPolicyFactory _policy)
    {
      var allOrgIds = GetAllSolutions(_policy).Select(soln => soln.OrganisationId).Distinct().ToList();
      var orgDatastore = new OrganisationsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<OrganisationsDatastore>>().Object, _policy);
      var allOrgs = allOrgIds.Select(orgId => orgDatastore.ById(orgId)).ToList();

      return allOrgs;
    }

    public static List<Capabilities> GetAllCapabilities(ISyncPolicyFactory _policy)
    {
      var datastore = new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<CapabilitiesDatastore>>().Object, _policy);
      var datas = datastore.GetAll().ToList();

      return datas;
    }

    public static List<Standards> GetAllStandards(ISyncPolicyFactory _policy)
    {
      var datastore = new StandardsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsDatastore>>().Object, _policy);
      var datas = datastore.GetAll().ToList();

      return datas;
    }

  }
}
