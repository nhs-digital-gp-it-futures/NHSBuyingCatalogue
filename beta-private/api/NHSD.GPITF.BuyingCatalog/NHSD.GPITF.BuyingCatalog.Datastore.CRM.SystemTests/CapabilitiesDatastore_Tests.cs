using FluentAssertions;
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
      Assert.DoesNotThrow(() => new CapabilitiesDatastore(_crmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void GetAll_ReturnsData()
    {
      var datastore = new CapabilitiesDatastore(_crmConnectionFactory, _logger, _policy);

      var datas = datastore.GetAll();

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ById_UnknownId_ReturnsNull()
    {
      var datastore = new CapabilitiesDatastore(_crmConnectionFactory, _logger, _policy);

      var data = datastore.ById(Guid.NewGuid().ToString());

      data.Should().BeNull();
    }

    [Test]
    public void ById_KnownId_ReturnsData()
    {
      var datastore = new CapabilitiesDatastore(_crmConnectionFactory, _logger, _policy);
      var allData = datastore.GetAll().ToList();

      var allDataById = allData.Select(data => datastore.ById(data.Id));

      allDataById.Should().BeEquivalentTo(allData);
    }

    [Test]
    public void ByIds_KnownIds_ReturnsData()
    {
      var datastore = new CapabilitiesDatastore(_crmConnectionFactory, _logger, _policy);
      var allData = datastore.GetAll();
      var allDataIds = allData.Select(data => data.Id);

      var allDataByIds = datastore.ByIds(allDataIds);

      allDataByIds.Should().BeEquivalentTo(allData);
    }

    [Test]
    public void ByFramework_KnownIds_ReturnsData()
    {
      var otherDatastore = new FrameworksDatastore(_crmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      var others = otherDatastore.GetAll();
      var datastore = new CapabilitiesDatastore(_crmConnectionFactory, _logger, _policy);

      var datas = others.ToList().SelectMany(other => datastore.ByFramework(other.Id));

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void ByStandard_KnownIds_ReturnsData()
    {
      var otherDatastore = new StandardsDatastore(_crmConnectionFactory, new Mock<ILogger<StandardsDatastore>>().Object, _policy);
      var others = otherDatastore.GetAll();
      var datastore = new CapabilitiesDatastore(_crmConnectionFactory, _logger, _policy);

      var datas = others.ToList().SelectMany(other => datastore.ByStandard(other.Id, true));

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }
  }
}
