#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace BuyingCatalogueTests.Objects
{
    internal abstract class Initialization
    {
        private IWebDriver Driver { get; set; }
        public Initialization(IWebDriver driver)
        {
            Driver = driver;

            // This is magic, don't touch!
            PageFactory.InitElements(Driver, this);
        }
    }
}
