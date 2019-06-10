using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace BuyingCatalogueTests.PageActions
{
    internal sealed class CapabilitiesEvidenceActions : Initialize
    {
        string contactMessage;

        public CapabilitiesEvidenceActions(IWebDriver driver) : base(driver)
        {
        }

        internal void WaitForPageDisplayed()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#capability-assessment-form fieldset.collapsible")));
        }

        internal void ClickSaveAndReview()
        {
            capEvidenceObjects.SaveAndReviewButton.Click();
        }

        internal void SelectForEachCapability(string labelText)
        {
            capEvidenceObjects.CapabilitiesAccordian = new Objects.CapabilitiesEvidenceObjects(_driver).CapabilitiesAccordian;

            var firstCap = capEvidenceObjects.CapabilitiesAccordian[0];

            firstCap.FindElements(By.TagName("label")).Single(s => s.Text == labelText).Click();

            _wait.Until(ExpectedConditions.ElementToBeClickable(firstCap.FindElement(By.CssSelector("fieldset.evidence-field h3"))));

            if(capEvidenceObjects.CapabilitiesAccordian.Count > 1)
            { 
                for(int i = 1; i < capEvidenceObjects.CapabilitiesAccordian.Count; i++)
                {
                    var capability = capEvidenceObjects.CapabilitiesAccordian[i];
                    capability.Click();
                    capability.FindElements(By.TagName("label")).Single(s => s.Text == labelText).Click();
                    _wait.Until(ExpectedConditions.ElementToBeClickable(capability.FindElement(By.CssSelector("fieldset.evidence-field h3"))));
                }
            }
        }

        internal void VerifyCorrectErrorMessagesDisplayed()
        {
            capEvidenceObjects.CapabilityErrors = new Objects.CapabilitiesEvidenceObjects(_driver).CapabilityErrors;

            capEvidenceObjects.CapabilityErrors.Count.Should().Be(capEvidenceObjects.CapabilitiesAccordian.Count);
        }

        internal void ClickLabelFirstCap(string labelText)
        {
            capEvidenceObjects.CapabilitiesAccordian = new Objects.CapabilitiesEvidenceObjects(_driver).CapabilitiesAccordian;

            var firstCap = capEvidenceObjects.CapabilitiesAccordian[0];

            firstCap.FindElements(By.TagName("label")).Single(s => s.Text == labelText).Click();

            contactMessage = firstCap.FindElement(By.CssSelector("fieldset.evidence-field legend")).Text;
        }

        internal void VerifyContactMessage(string expected)
        {
            contactMessage.Should().Be(expected);
        }
    }
}
