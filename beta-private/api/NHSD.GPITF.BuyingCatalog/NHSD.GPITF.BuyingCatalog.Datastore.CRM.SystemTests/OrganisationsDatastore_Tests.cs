using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Logic;
using NUnit.Framework;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class OrganisationsDatastore_Tests : DatastoreBase_Tests<OrganisationsDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new OrganisationsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void ById_ReturnsData()
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll();
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var allSolns = frameworks.ToList().SelectMany(fw => solnDatastore.ByFramework(fw.Id));
      var ids = allSolns.Select(soln => soln.OrganisationId).Distinct();

      var datastore = new OrganisationsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = ids.ToList().Select(id => datastore.ById(id));

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ByContact_ReturnsData()
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll();
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var allSolns = frameworks.ToList().SelectMany(fw => solnDatastore.ByFramework(fw.Id));
      var allOrgIds = allSolns.Select(soln => soln.OrganisationId).Distinct();
      var contactsDatastore = new ContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<ContactsDatastore>>().Object, _policy);
      var allContacts = allOrgIds.ToList().SelectMany(orgId => contactsDatastore.ByOrganisation(orgId));
      var datastore = new OrganisationsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = allContacts.ToList().Select(contact => datastore.ByContact(contact.Id));

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }
  }
}
