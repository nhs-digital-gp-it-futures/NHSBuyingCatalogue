using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Collections.Generic;

namespace BuyingCatalogueTests.Objects
{
    internal class StandardsEvidenceObjects : Initialization
    {
        public StandardsEvidenceObjects(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.CssSelector, Using = "#compliance table:nth-of-type(1) tr.standard")]
        internal IList<IWebElement> AssociatedStandards { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#compliance table:nth-of-type(2) tr.standard")]
        internal IList<IWebElement> OverarchingStandards { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#compliance header.page-main-heading h1")]
        internal IWebElement PageHeader { get; set; }
    }
}