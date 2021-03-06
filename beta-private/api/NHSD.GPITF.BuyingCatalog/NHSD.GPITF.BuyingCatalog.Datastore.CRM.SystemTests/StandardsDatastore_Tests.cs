﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Logic;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class StandardsDatastore_Tests : DatastoreBase_Tests<StandardsDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new StandardsDatastore(DatastoreBaseSetup.StandardsDatastore, _logger, _policy, _config, new Mock<ILongTermCache>().Object));
    }

    [Test]
    public void GetAll_ReturnsData()
    {
      var datastore = new StandardsDatastore(DatastoreBaseSetup.StandardsDatastore, _logger, _policy, _config, new Mock<ILongTermCache>().Object);

      var datas = datastore.GetAll().ToList();

      datas.Should().NotBeEmpty();
      datas.ForEach(data => data.Should().NotBeNull());
      datas.ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ById_UnknownId_ReturnsNull()
    {
      var datastore = new StandardsDatastore(DatastoreBaseSetup.StandardsDatastore, _logger, _policy, _config, new Mock<ILongTermCache>().Object);

      var data = datastore.ById(Guid.NewGuid().ToString());

      data.Should().BeNull();
    }

    [Test]
    public void ById_KnownId_ReturnsData()
    {
      var datastore = new StandardsDatastore(DatastoreBaseSetup.StandardsDatastore, _logger, _policy, _config, new Mock<ILongTermCache>().Object);
      var allData = datastore.GetAll().ToList();

      var allDataById = allData.Select(data => datastore.ById(data.Id)).ToList();

      allDataById.Should().BeEquivalentTo(allData);
    }

    [Test]
    public void ByIds_KnownIds_ReturnsData()
    {
      var datastore = new StandardsDatastore(DatastoreBaseSetup.StandardsDatastore, _logger, _policy, _config, new Mock<ILongTermCache>().Object);
      var allData = datastore.GetAll().ToList();
      var allDataIds = allData.Select(data => data.Id);

      var allDataByIds = datastore.ByIds(allDataIds).ToList();

      allDataByIds.Should().BeEquivalentTo(allData);
    }

    [Test]
    public void ByFramework_KnownIds_ReturnsData()
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.FrameworksDatastore, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll().ToList();
      var datastore = new StandardsDatastore(DatastoreBaseSetup.StandardsDatastore, _logger, _policy, _config, new Mock<ILongTermCache>().Object);

      var datas = frameworks.SelectMany(fw => datastore.ByFramework(fw.Id)).ToList();

      datas.Should().NotBeEmpty();
      datas.ForEach(data => data.Should().NotBeNull());
      datas.ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ByCapability_KnownIds_ReturnsData()
    {
      var capsDatastore = new CapabilitiesDatastore(DatastoreBaseSetup.CapabilitiesDatastore, new Mock<ILogger<CapabilitiesDatastore>>().Object, _policy, _config, new Mock<ILongTermCache>().Object);
      var caps = capsDatastore.GetAll().ToList();
      var datastore = new StandardsDatastore(DatastoreBaseSetup.StandardsDatastore, _logger, _policy, _config, new Mock<ILongTermCache>().Object);

      var datas = caps.SelectMany(cap => datastore.ByCapability(cap.Id, true)).ToList();

      datas.Should().NotBeEmpty();
      datas.ForEach(data => data.Should().NotBeNull());
      datas.ForEach(data => Verifier.Verify(data));
    }
  }
}
