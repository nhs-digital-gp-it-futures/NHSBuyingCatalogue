﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Logic;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class CapabilitiesDatastore_Tests : DatastoreBase_Tests<CapabilitiesDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void GetAll_ReturnsData()
    {
      var datastore = new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = datastore.GetAll().ToList();

      datas.Should().NotBeEmpty();
      datas.ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ById_UnknownId_ReturnsNull()
    {
      var datastore = new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var data = datastore.ById(Guid.NewGuid().ToString());

      data.Should().BeNull();
    }

    [Test]
    public void ById_KnownId_ReturnsData()
    {
      var datastore = new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);
      var allData = datastore.GetAll().ToList();

      var allDataById = allData.Select(data => datastore.ById(data.Id)).ToList();

      allDataById.Should().BeEquivalentTo(allData);
    }

    [Test]
    public void ByIds_KnownIds_ReturnsData()
    {
      var datastore = new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);
      var allData = datastore.GetAll();
      var allDataIds = allData.Select(data => data.Id).ToList();

      var allDataByIds = datastore.ByIds(allDataIds).ToList();

      allDataByIds.Should().BeEquivalentTo(allData);
    }

    [Test]
    public void ByFramework_KnownIds_ReturnsData()
    {
      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var frameworks = frameworksDatastore.GetAll().ToList();
      var datastore = new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = frameworks.SelectMany(other => datastore.ByFramework(other.Id)).ToList();

      datas.Should().NotBeEmpty();
      datas.ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ByStandard_KnownIds_ReturnsData()
    {
      var stdsDatastore = new StandardsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsDatastore>>().Object, _policy);
      var stds = stdsDatastore.GetAll().ToList();
      var datastore = new CapabilitiesDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = stds.SelectMany(other => datastore.ByStandard(other.Id, true)).ToList();

      datas.Should().NotBeEmpty();
      datas.ForEach(data => Verifier.Verify(data));
    }
  }
}
