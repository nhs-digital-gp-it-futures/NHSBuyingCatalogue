using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class ContactsDatastore_Tests : DatastoreBase_Tests<ContactsDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new ContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void ByEmail_ReturnsData()
    {
      var emails = GetAll().Select(ent => ent.EmailAddress1);
      var datastore = new ContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = emails.ToList().Select(email => datastore.ByEmail(email));

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ById_ReturnsData()
    {
      var ids = GetAll().Select(ent => ent.EmailAddress1);
      var datastore = new ContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = ids.ToList().Select(id => datastore.ById(id));

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ByOrganisation_ReturnsData()
    {
      var datas = GetAll();

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }

    private IEnumerable<Contacts> GetAll()
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll();
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var allSolns = frameworks.ToList().SelectMany(fw => solnDatastore.ByFramework(fw.Id));
      var allOrgIds = allSolns.Select(soln => soln.Id).Distinct();
      var datastore = new ContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = allOrgIds.ToList().SelectMany(orgId => datastore.ByOrganisation(orgId));

      return datas;
    }
  }
}
