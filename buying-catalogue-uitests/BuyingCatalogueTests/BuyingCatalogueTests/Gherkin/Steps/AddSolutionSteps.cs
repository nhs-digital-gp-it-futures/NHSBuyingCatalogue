#pragma warning disable CS1591
using BuyingCatalogueTests.utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    [Binding]
    public partial class SpecFlowSteps
    {
        #region Givens
        [Given(@"I click the `Add a solution` button")]
        public void GivenIClickTheAddASolutionButton()
        {
            homePageActions.AddASolution();
        }

        [Given(@"I click the `Start` link below the Basic Details section")]
        public void GivenIClickTheStartLinkBelowTheBasicDetailsSection()
        {
            onboardingActions.BasicDetailsStartClick();
        }

        [Given(@"I complete the Basic Details Section")]
        public void GivenICompleteTheBasicDetailsSection()
        {
            string name = string.Join(" ", faker.Lorem.Words(new Random().Next(1, 10)));
            name = name.Length > 60 ? name.Substring(0, 60) : name;

            string description = faker.Rant.Review();

            string versionNumber = faker.Random.Number(1, 11).ToString() + "." + faker.Random.Number(1, 10).ToString();

            basicDetailsActions.EnterSolutionName(name);
            basicDetailsActions.EnterSolutionDescription(description);
            basicDetailsActions.EnterVersionNumber(versionNumber);
        }

        [Given(@"I complete the Lead Contact Details section")]
        public void GivenICompleteTheLeadContactDetailsSection()
        {
            basicDetailsActions.OpenLeadContactDetailsSection();

            ContactDetails details = TestDataGenerator.GenerateTestContact();

            basicDetailsActions.EnterLeadContactDetails(details);
        }

        [Given(@"I save the name and version of an existing solution")]
        public void GivenISaveTheNameAndVersionOfAnExistingSolution()
        {
            nameVersion = homePageActions.GetRandomSolutionNameAndVersion();
        }

        [Given(@"I enter the existing solution name and version")]
        public void GivenIEnterTheExistingSolutionNameAndVersion()
        {
            // Complete the Solution Name, Description and Version (using the previously created details above)
            basicDetailsActions.EnterSolutionName(nameVersion[0]);
            basicDetailsActions.EnterVersionNumber(nameVersion[1]);
            basicDetailsActions.EnterSolutionDescription(faker.Rant.Review());
        }

        [Given(@"I click Save & Continue the (.*) page is displayed")]
        public void SaveAndContinuePageDisplayed(string pageTitle)
        {
            basicDetailsActions.SaveAndContinue(pageTitle);
        }
        #endregion

        #region Whens
        [When(@"I click Save without filling in the Solution Name Field")]
        public void WhenIClickSaveWithoutFillingInTheSolutionNameField()
        {
            errorMessage = basicDetailsActions.SaveWithEmptySolutionName();
        }

        [When(@"I click Save without filling in the Solution Description Field")]
        public void WhenIClickSaveWithoutFillingInTheSolutionDescriptionField()
        {
            errorMessage = basicDetailsActions.SaveWithEmptySolutionDescription();
        }

        [When(@"I click save without completing the Lead Contact details sections")]
        public void WhenIClickSaveWithoutCompletingTheLeadContactDetailsSections()
        {
            errors = basicDetailsActions.SaveMissingAllContactDetails();
        }

        [When(@"I enter a name with more than '(.*)' characters")]
        public void WhenIEnterANameWithMoreThanCharacters(int max)
        {
            string name = faker.Lorem.Letter(max + 1);
            basicDetailsActions.EnterSolutionName(name);
        }

        [When(@"I click Save expecting a Solution Name error")]
        public void WhenIClickSave()
        {
            basicDetailsActions.SaveSolution();

            errorMessage = basicDetailsActions.GetSolutionNameError();
        }

        [When(@"I enter a description with more than '(.*)' characters")]
        public void WhenIEnterADescriptionWithMoreThanCharacters(int max)
        {
            string description = faker.Lorem.Letter(max + 1);
            basicDetailsActions.EnterSolutionDescription(description);
        }

        [When(@"I click Save expecting a Solution Description error")]
        public void WhenIClickSaveExpectingASolutionDescriptionError()
        {
            basicDetailsActions.SaveSolution();

            errorMessage = basicDetailsActions.GetSolutionDescriptionError();
        }
        #endregion

        #region Thens
        [Then(@"I should see the onboarding page with a title of '(.*)'")]
        public void ThenIShouldSeeTheOnboardingPage(string title)
        {
            onboardingActions.GetMainHeaderText().Should().Be(title);
        }

        [Then(@"The .* Error should be '(.*)'")]
        public void ThenTheErrorShouldBe(string expected)
        {
            errorMessage.Should().Be(expected);
        }

        [Then(@"I should be logged into the system")]

        public void ThenIShouldBeLoggedIntoTheSystem()

        {
            authActions.GetLoginOutLinkText().Should().Be("Log Out");
        }

        [Then(@"I should be logged out of the system")]
        public void ThenIShouldBeLoggedOutOfTheSystem()
        {
            authActions.GetLoginOutLinkText().Should().Be("Log in");
        }

        [Then(@"The correct errors for the Lead Contact are displayed")]
        public void ThenTheCorrectErrorsForTheLeadContactAreDisplayed()
        {
            List<string> expectedErrors = new List<string>
            {
                "Contact first name is missing",
                "Contact last name is missing",
                "Contact email is missing",
                "Contact phone number is missing"
            };

            errors.Should().Equal(expectedErrors);
        }

        [Then(@"I can add a lot of additional contacts")]
        public void ThenICanAddALotOfAdditionalContacts()
        {
            basicDetailsActions.AddAdditionalContacts(new Random().Next(50, 100));
        }
        #endregion     
    }
}
