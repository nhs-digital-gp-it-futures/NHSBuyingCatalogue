#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Linq;

namespace BuyingCatalogueTests.PageActions
{
    internal sealed class SolutionsOnboardingActions : Initialize
    {
        public SolutionsOnboardingActions(IWebDriver driver) : base(driver)
        {
        }

        internal void BasicDetailsStartClick()
        {
            // Reinitialize the list of onboarding stages
            onboardingObjects.OnboardingStages = new Objects.SolutionsOnboardingObjects(_driver).OnboardingStages;

            var stage = onboardingObjects.OnboardingStages.Single(s => s.FindElement(By.ClassName("title")).Text.Contains("basic details"));

            stage.FindElement(By.CssSelector(".action a")).Click();

            _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.SolutionName));
        }

        internal void StandardsEvidenceStartClick()
        {
            // Reinitialize the list of onboarding stages
            onboardingObjects.OnboardingStages = new Objects.SolutionsOnboardingObjects(_driver).OnboardingStages;

            var stage = onboardingObjects.OnboardingStages.Single(s => s.FindElement(By.ClassName("title")).Text.Contains("Solution's Standards"));

            stage.FindElement(By.CssSelector(".action a")).Click();

            _wait.Until(ExpectedConditions.ElementToBeClickable(standardsEvidenceObjects.PageHeader));
        }

        internal void SolutionCapabilitiesStartClick()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(GetByOnboardingObject(nameof(onboardingObjects.OnboardingStages))));

            var stage = onboardingObjects.OnboardingStages[1];

            stage.FindElement(By.CssSelector(".action a")).Click();

            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#capability-assessment-form fieldset.collapsible")));
        }

        internal void VerifySolutionName(string solutionName)
        {
            string shortName = solutionName.Split('|')[0].Trim();

            onboardingObjects.SolutionName.Text.Should().Contain(shortName);
        }

        internal string ViewSolutionDetails()
        {
            var stage = onboardingObjects.OnboardingStages.Single(s => s.FindElement(By.ClassName("title")).Text.Contains("basic details"));

            string actionText = stage.FindElement(By.CssSelector(".action a")).Text;
            stage.FindElement(By.CssSelector(".action a")).Click();

            if (actionText == "Edit")
            {
                _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.SolutionName));
            }
            else
            {
                _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.ReadOnlySolutionDetails));
            }

            return actionText;
        }

        internal string GetMainHeaderText()
        {
            return commonObjects.PageHeader.Text;
        }
    }
}
