using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Collections.Generic;

namespace BuyingCatalogueTests.Objects
{
    internal class AddCapabilitiesObjects : Initialization
    {
        public AddCapabilitiesObjects(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.CssSelector, Using = "#capabilities-core ul li.capability")]
        internal IList<IWebElement> FoundationCapabilities { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#capabilities-non-core ul li.capability")]
        internal IList<IWebElement> OtherCapabilities { get; set; }

        [FindsBy(How = How.CssSelector, Using = "span.capability-count span.counter")]
        internal IWebElement CapabilitiesSelectedCounter { get; set; }

        [FindsBy(How = How.CssSelector, Using = "section.associated ul li a")]
        internal IList<IWebElement> AssociatedStandards { get; set; }

        [FindsBy(How = How.CssSelector, Using = "section.capabilities ul.capabilities li.selected")]
        internal IList<IWebElement> AddedCapabilities { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div#submit-controls input")]
        internal IWebElement SaveAndComplete { get; set; }

        [FindsBy(How = How.Id, Using = "error-capabilities")]
        internal IWebElement ErrorMessage { get; set; }
    }
}
