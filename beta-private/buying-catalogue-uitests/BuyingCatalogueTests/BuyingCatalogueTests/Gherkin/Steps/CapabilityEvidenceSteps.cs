#pragma warning disable CS1591

using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    public sealed partial class SpecFlowSteps
    {
        [Given(@"I click the `Start` link below the Capabilities Evidence section")]
        public void GivenIClickTheStartLinkBelowTheCapabilitiesEvidenceSection()
        {
            onboardingActions.SolutionCapabilitiesStartClick();

            capEvidenceActions.WaitForPageDisplayed();
        }

        [When(@"I click the 'Save and review' button")]
        public void WhenIClickTheSaveAndReviewButton()
        {
            capEvidenceActions.ClickSaveAndReview();
        }

        [Then(@"I can click '(.*)'")]
        public void ThenICanClick(string labelText)
        {
            capEvidenceActions.ClickLabelFirstCap(labelText);
        }

        [Then(@"A message will show '(.*)'")]
        public void ThenAMessageWillShow(string expected)
        {
            capEvidenceActions.VerifyContactMessage(expected);
        }

        [Then(@"An error for each capability should be displayed")]
        public void ThenAnErrorForEachCapabilityShouldBeDisplayed()
        {
            capEvidenceActions.VerifyCorrectErrorMessagesDisplayed();
        }
    }
}
