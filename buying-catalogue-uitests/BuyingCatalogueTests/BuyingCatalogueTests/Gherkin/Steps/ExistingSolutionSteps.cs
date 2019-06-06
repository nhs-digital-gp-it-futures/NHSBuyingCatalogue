#pragma warning disable CS1591

using FluentAssertions;
using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    public sealed partial class SpecFlowSteps
    {
        [Given(@"I have opened a solution")]
        public void GivenIHaveOpenedASolution()
        {
            solutionName = homePageActions.OpenRandomSolution();
        }

        [When(@"I click on the solution details")]
        public void WhenIClickOnTheSolutionDetails()
        {
            editView = onboardingActions.ViewSolutionDetails();
        }

        [Then(@"the solution title should be displayed")]
        public void ThenTheSolutionTitleShouldBeDisplayed()
        {
            onboardingActions.VerifySolutionName(solutionName);
        }

        [Then(@"the solution title should be displayed in the details section")]
        public void ThenTheSolutionTitleShouldBeDisplayedInTheDetailsSection()
        {
            if (editView == "Edit")
            {
                basicDetailsActions.GetSolutionName().Should().Be(solutionName);
            }
            else
            {
                basicDetailsActions.GetSolutionDetails().Should().Contain(solutionName);
            }
        }
    }
}
