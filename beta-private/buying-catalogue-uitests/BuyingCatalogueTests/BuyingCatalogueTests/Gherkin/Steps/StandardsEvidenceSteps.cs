#pragma warning disable CS1591

using FluentAssertions;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    public sealed partial class SpecFlowSteps
    {
        private string standardName;

        [Given(@"A solution has been created")]
        public void GivenASolutionHasBeenCreated()
        {
            GivenIClickTheAddASolutionButton();
            GivenIClickTheStartLinkBelowTheBasicDetailsSection();
            GivenICompleteAllBasicDetails();
            GivenTheListOfCapabilitiesIsDisplayed();
            WhenISelectAnOtherCapabilityAndSaveItsStandards();
            ClickSaveAndComplete();   
        }

        [Given(@"the capability evidence has been submitted")]
        public void GivenTheCapabilityEvidenceHasBeenSubmitted()
        {
            GivenIClickTheStartLinkBelowTheCapabilitiesEvidenceSection();
            ThenICanClick("Schedule live witness demonstration");
            capEvidenceActions.ClickSaveAndReview();
            capEvidenceActions.SubmitEvidence();
        }

        [Given(@"I click the Start button below the Standards Evidence section")]
        public void GivenIClickTheStartButtonBelowTheStandardsEvidenceSection()
        {
            onboardingActions.StandardsEvidenceStartClick();
        }

        [Then(@"the page title should be '(.*)'")]
        public void ThenThePageTitleShouldBe(string expectedTitle)
        {
            standardsEvidenceActions.GetStandardTitle().Should().Be(expectedTitle);
        }

        [Then(@"two tables should be displayed for the standards")]
        public void ThenTwoTablesShouldBeDisplayedForTheStandards()
        {
            standardsEvidenceActions.GetStandardsTablesCount().Should().Be(2);
        }

        [Then(@"all standards from the selected capabilities are displayed")]
        public void ThenAllStandardsFromTheSelectedCapabilitiesAreDisplayed()
        {
            var allStandards = new List<string>();
            allStandards.AddRange(standardsEvidenceActions.GetAssociatedStandards());

            allStandards.Count.Should().Be(standards.Count);
        }

        [When(@"I click the Help - What are the associated Standards link")]
        public void WhenIClickTheHelp_WhatAreTheAssociatedStandardsLink()
        {
            standardsEvidenceActions.RevealHelpContents();
        }

        [Then(@"the help text should be correct")]
        public void ThenTheHelpTextShouldBeCorrect()
        {
            standardsEvidenceActions.GetHelpContents().Should().Be("Associated Standards relate to one or more of the Capabilities selected. They include Capability-Specific Standards and Context-Specific Standards.");
        }

        [When(@"I click on a standard")]
        public void WhenIClickOnAStandard()
        {
            standardName = standardsEvidenceActions.ClickStandard();
        }

        [Then(@"the correct standard page should be displayed")]
        public void ThenTheCorrectStandardPageShouldBeDisplayed()
        {
            standardsEvidenceActions.GetStandardTitle().Should().Be(standardName);
        }
    }
}
