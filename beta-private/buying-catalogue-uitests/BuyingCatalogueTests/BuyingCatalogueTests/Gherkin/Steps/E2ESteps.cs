#pragma warning disable CS1591

using BuyingCatalogueTests.utils;
using System;
using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    public sealed partial class SpecFlowSteps
    {
        [Given(@"I complete all basic details")]
        public void GivenICompleteAllBasicDetails()
        {
            string name = string.Join(" ", faker.Lorem.Words(new Random().Next(1, 10)));
            name = name.Length > 60 ? name.Substring(0, 60) : name;

            string description = faker.Rant.Review();

            string versionNumber = faker.Random.Number(1, 11).ToString() + "." + faker.Random.Number(1, 10).ToString();

            solutionName = $"{name} | {versionNumber}";

            basicDetailsActions.EnterSolutionName(name);
            basicDetailsActions.EnterSolutionDescription(description);
            basicDetailsActions.EnterVersionNumber(versionNumber);

            basicDetailsActions.OpenLeadContactDetailsSection();

            ContactDetails details = TestDataGenerator.GenerateTestContact();

            basicDetailsActions.EnterLeadContactDetails(details);

            basicDetailsActions.SaveAndContinue("Which Capabilities does your Solution provide?*");
        }

        [Then(@"I should see the onboarding page with the name of the solution")]
        public void ThenIShouldSeeTheOnboardingPageWithTheNameOfTheSolution()
        {
            onboardingActions.VerifySolutionName(solutionName);            
        }
    }
}
