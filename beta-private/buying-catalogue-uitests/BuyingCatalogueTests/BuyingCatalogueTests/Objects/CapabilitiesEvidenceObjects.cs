using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace BuyingCatalogueTests.Objects
{
    internal sealed class CapabilitiesEvidenceObjects : Initialization
    {
        public CapabilitiesEvidenceObjects(IWebDriver driver) : base(driver)
        {
        }        

        [FindsBy(How = How.CssSelector, Using = "#capability-assessment-form fieldset.collapsible")]
        internal IList<IWebElement> CapabilitiesAccordian { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#errors ul li")]
        internal IList<IWebElement> CapabilityErrors { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#capability-assessment-form button")]
        internal IWebElement SaveAndReviewButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#capability-submission-confirmation header h1")]
        internal IWebElement ReviewPageTitle { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#capability-assessment-form button[type=submit]")]
        internal IWebElement ReviewPageSubmit { get; set; }
    }
}
