#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Collections.Generic;

namespace BuyingCatalogueTests.Objects
{
    internal class SolutionsOnboardingObjects : Initialization
    {
        public SolutionsOnboardingObjects(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.ClassName, Using = "onboarding-stages")]
        public IList<IWebElement> OnboardingStages { get; set; }

        [FindsBy(How = How.CssSelector, Using = "nav.active-form span.title")]
        public IWebElement SolutionName { get; set; }
    }
}
