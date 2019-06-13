#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;

namespace BuyingCatalogueTests.PageActions
{
    internal sealed class HomePageActions : Initialize
    {
        public HomePageActions(IWebDriver driver) : base(driver)
        {
        }

        internal void AddASolution()
        {
            homePageObjects.AddSolution.Click();

            _wait.Until(ExpectedConditions.ElementToBeClickable(GetByOnboardingObject(nameof(onboardingObjects.OnboardingStages))));
        }

        internal string OpenRandomSolution()
        {
            homePageObjects.Solutions = new Objects.HomePageObjects(_driver).Solutions;

            var solution = homePageObjects.Solutions[new Random().Next(homePageObjects.Solutions.Count)];

            var solutionName = solution.Text;

            solution.Click();

            _wait.Until(ExpectedConditions.ElementToBeClickable(GetByOnboardingObject(nameof(onboardingObjects.OnboardingStages))));

            return solutionName.Split('|')[0].Trim();
        }

        internal IWebElement GetSolutionInList(string solutionName)
        {
            homePageObjects.Solutions = new Objects.HomePageObjects(_driver).Solutions;

            return homePageObjects.Solutions.Single(s => s.Text.Contains(solutionName));
        }

        internal string[] GetRandomSolutionNameAndVersion()
        {
            homePageObjects.Solutions = new Objects.HomePageObjects(_driver).Solutions;

            var solution = homePageObjects.Solutions[new Random().Next(homePageObjects.Solutions.Count)];

            var solutionName = solution.Text;

            // Hacky way of splitting then trimming a string to an array
            return solutionName.Split('|').ToList().Select(s => s.Trim()).ToArray();
        }
    }
}
