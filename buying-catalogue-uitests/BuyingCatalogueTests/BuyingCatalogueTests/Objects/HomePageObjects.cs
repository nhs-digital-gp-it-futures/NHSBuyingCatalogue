#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Collections.Generic;

namespace BuyingCatalogueTests.Objects
{
    internal sealed class HomePageObjects : Initialization
    {
        public HomePageObjects(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.Id, Using = "add-new-solution")]
        public IWebElement AddSolution { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#solutions-onboarding tr a")]
        public IList<IWebElement> Solutions { get; set; }
    }
}
