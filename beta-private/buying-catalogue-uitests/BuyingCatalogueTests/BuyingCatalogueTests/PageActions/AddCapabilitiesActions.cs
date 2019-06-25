using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BuyingCatalogueTests.PageActions
{
    internal sealed class AddCapabilitiesActions : Initialize
    {
        readonly List<string> capabilities = new List<string>();

        public AddCapabilitiesActions(IWebDriver driver) : base(driver)
        {
        }

        internal void WaitForCapabilities()
        {
            // Refresh the lists of capabilities
            addCapabilitiesObjects.FoundationCapabilities = new Objects.AddCapabilitiesObjects(_driver).FoundationCapabilities;
            addCapabilitiesObjects.OtherCapabilities = new Objects.AddCapabilitiesObjects(_driver).OtherCapabilities;
        }

        internal List<string> SelectRandomFoundationCapability()
        {
            int index = new Random().Next(addCapabilitiesObjects.FoundationCapabilities.Count);

            var capability = addCapabilitiesObjects.FoundationCapabilities[index];

            SelectCapability(capability);

            capabilities.Add(capability.FindElement(By.TagName("h3")).Text);

            return capability.FindElements(By.CssSelector("section.standards ul:nth-child(1) li a")).Select(s => s.Text).ToList();
        }
        internal int GetCounter()
        {
            return int.Parse(addCapabilitiesObjects.CapabilitiesSelectedCounter.Text);
        }

        internal IEnumerable<string> GetAddedCapabilitiesStandards()
        {
            addCapabilitiesObjects.AssociatedStandards = new Objects.AddCapabilitiesObjects(_driver).AssociatedStandards;
            List<string> standards = addCapabilitiesObjects.AssociatedStandards.Select(s => s.Text).ToList();

            return standards;
        }

        internal IList<string> GetAllStandards()
        {
            addCapabilitiesObjects.AddedStandards = new Objects.AddCapabilitiesObjects(_driver).AddedStandards;
            var standards = addCapabilitiesObjects.AddedStandards.Select(s => s.Text).ToList();

            return standards;
        }

        internal IList<string> SelectRandomOtherCapability()
        {
            int index = new Random().Next(addCapabilitiesObjects.OtherCapabilities.Count);

            var capability = addCapabilitiesObjects.OtherCapabilities[index];

            SelectCapability(capability);

            capabilities.Add(capability.FindElement(By.TagName("h3")).Text);

            var standards = capability.FindElements(By.CssSelector("section.standards ul:nth-of-type(1) li"));

            List<string> standardsText = new List<string>();

            foreach(var text in standards)
            {
                Console.WriteLine(text);
                standardsText.Add(text.Text);
            }

            return standardsText;
        }

        internal string GetErrorMessage()
        {
            return addCapabilitiesObjects.ErrorMessage.Text;
        }

        internal void SaveAndComplete()
        {
            addCapabilitiesObjects.SaveAndComplete.Click();
        }

        internal void CheckCapabilitesHaveAddedCorrectly()
        {
            addCapabilitiesObjects.AddedCapabilities = new Objects.AddCapabilitiesObjects(_driver).AddedCapabilities;

            var added = addCapabilitiesObjects.AddedCapabilities.Select(s => s.Text);           

            added.Should().Contain(capabilities);
        }

        private void SelectCapability(IWebElement element)
        {
            new Actions(_driver).MoveToElement(element).Build().Perform();

            new Actions(_driver).Click().Build().Perform();

            // TODO: Replace
            // Don't like this, must find a better way
            Thread.Sleep(1000);

            element.FindElement(By.CssSelector("div.controls label.unchecked.button")).Click();
        }
    }
}
