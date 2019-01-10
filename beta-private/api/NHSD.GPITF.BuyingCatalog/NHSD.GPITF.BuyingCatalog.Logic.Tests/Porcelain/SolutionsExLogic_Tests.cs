using FluentAssertions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Logic.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests.Porcelain
{
  [TestFixture]
  public sealed class SolutionsExLogic_Tests
  {
    private Mock<ISolutionsExDatastore> _datastore;
    private Mock<IContactsDatastore> _contacts;
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsExValidator> _validator;
    private Mock<ISolutionsExFilter> _filter;
    private Mock<IEvidenceBlobStoreLogic> _evidenceBlobStoreLogic;

    [SetUp]
    public void SetUp()
    {
      _datastore = new Mock<ISolutionsExDatastore>();
      _contacts = new Mock<IContactsDatastore>();
      _context = new Mock<IHttpContextAccessor>();
      _validator = new Mock<ISolutionsExValidator>();
      _filter = new Mock<ISolutionsExFilter>();
      _evidenceBlobStoreLogic = new Mock<IEvidenceBlobStoreLogic>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionsExLogic(_datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object));
    }

    [Test]
    public void Update_CallsValidator_WithRuleset()
    {
      var logic = new SolutionsExLogic(_datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var solnEx = Creator.GetSolutionEx();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _validator.Verify(x => x.ValidateAndThrowEx(
        It.Is<SolutionEx>(sex => sex == solnEx),
        It.Is<string>(rs => rs == nameof(ISolutionsExLogic.Update))), Times.Once());
    }

    [TestCase(SolutionStatus.Registered)]
    public void Update_CallsPrepareForSolution_WhenRegistered(SolutionStatus status)
    {
      var logic = new SolutionsExLogic(_datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution(status: status);
      var solnEx = Creator.GetSolutionEx(soln: soln);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _evidenceBlobStoreLogic.Verify(x => x.PrepareForSolution(soln.Id), Times.Once);
    }

    [TestCase(SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Draft)]
    [TestCase(SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Approved)]
    public void Update_DoesNotCallPrepareForSolution_WhenNotRegistered(SolutionStatus status)
    {
      var logic = new SolutionsExLogic(_datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution(status: status);
      var solnEx = Creator.GetSolutionEx(soln: soln);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _evidenceBlobStoreLogic.Verify(x => x.PrepareForSolution(soln.Id), Times.Never);
    }

    [Test]
    public void Update_Sets_SolutionModifiedById()
    {
      var logic = new SolutionsExLogic(_datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      soln.ModifiedById.Should().Be(contact.Id);
    }

    [Test]
    public void Update_Sets_SolutionModifiedByOn()
    {
      var logic = new SolutionsExLogic(_datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution();
      soln.ModifiedOn = new DateTime(2006, 2, 20);
      var solnEx = Creator.GetSolutionEx(soln: soln);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      soln.ModifiedOn.Should().BeCloseTo(DateTime.UtcNow);
    }
  }
}
