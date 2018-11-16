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
    public static IEnumerable<Solutions> GetAllSolutions(ISyncPolicyFactory _policy)
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll();
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var allSolns = frameworks.ToList().SelectMany(fw => solnDatastore.ByFramework(fw.Id));
      var allOrgIds = allSolns.Select(soln => soln.Id).Distinct();
      var datastore = new ContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<ContactsDatastore>>().Object, _policy);

      var datas = allOrgIds.ToList().SelectMany(orgId => datastore.ByOrganisation(orgId));

      return allSolns;
    }

    public static IEnumerable<Capabilities> GetAllCapabilities(ISyncPolicyFactory _policy)
    {
      var datastore = new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<CapabilitiesDatastore>>().Object, _policy);
      var datas = datastore.GetAll();
      return datas;
    }

    public static IEnumerable<Standards> GetAllStandards(ISyncPolicyFactory _policy)
    {
      var datastore = new StandardsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsDatastore>>().Object, _policy);
      var datas = datastore.GetAll();
      return datas;
    }

  }
}
