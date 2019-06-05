using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace BuyingCatalogueTests.Objects
{
    internal class CommonObjects : Initialization
    {
        public CommonObjects(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.Id, Using = "home-link")]
        public IWebElement NHSLogo { get; set; }

        [FindsBy(How = How.CssSelector, Using = "input[type=submit]")]
        public IWebElement SaveAndSubmit { get; set; }

        [FindsBy(How = How.CssSelector, Using = "header.page-main-heading h1")]
        internal IWebElement PageHeader { get; set; }
    }
}