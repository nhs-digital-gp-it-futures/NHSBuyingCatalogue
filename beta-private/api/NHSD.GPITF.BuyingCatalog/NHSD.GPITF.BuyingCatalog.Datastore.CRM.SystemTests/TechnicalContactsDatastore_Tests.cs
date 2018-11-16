using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class TechnicalContactsDatastore_Tests : DatastoreBase_Tests<TechnicalContactsDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new TechnicalContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void BySolution_ReturnsData()
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll();
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var allSolns = frameworks.ToList().SelectMany(fw => solnDatastore.ByFramework(fw.Id));
      var datastore = new TechnicalContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = allSolns.ToList().SelectMany(soln => datastore.BySolution(soln.Id));

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void CRUD_Succeeds()
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll();
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var soln = frameworks.ToList().SelectMany(fw => solnDatastore.ByFramework(fw.Id)).First();
      var datastore = new TechnicalContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      // create
      var newEnt = new TechnicalContacts
      {
        Id = Guid.NewGuid().ToString(),
        SolutionId = soln.Id,
        ContactType = "Lead Contact",
        FirstName = "Steve",
        LastName = "Gray",
        EmailAddress = "steve.gray@nhs.net.uk",
        PhoneNumber = "1234567890"
      };
      Verifier.Verify(newEnt);
      var createdEnt = datastore.Create(newEnt);
      createdEnt.Should().BeEquivalentTo(newEnt, opt => opt.Excluding(tc => tc.Id));

      try
      {
        // update
        createdEnt.FirstName = "Jon";
        createdEnt.LastName = "Dough";
        datastore.Update(createdEnt);
        var updatedEnt = datastore.BySolution(soln.Id).Single(ent => ent.Id == createdEnt.Id);
        updatedEnt.Should().BeEquivalentTo(createdEnt);
      }
      finally
      {
        // delete
        datastore.Delete(createdEnt);
      }

      // delete
      var allEnts = datastore.BySolution(soln.Id);
      allEnts.Should().NotContain(ent => ent.Id == createdEnt.Id);
    }
  }
}
