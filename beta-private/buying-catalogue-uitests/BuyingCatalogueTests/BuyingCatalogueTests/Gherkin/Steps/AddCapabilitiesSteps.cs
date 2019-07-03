#pragma warning disable CS1591
using FluentAssertions;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    public sealed partial class SpecFlowSteps
    {
        List<string> standards = new List<string>();

        [Given(@"The list of capabilities is displayed")]
        public void GivenTheListOfCapabilitiesIsDisplayed()
        {
            addCapabilitiesActions.WaitForCapabilities();
        }

        [When(@"I select a foundational capability")]
        public void WhenISelectAFoundationalCapability()
        {
            addCapabilitiesActions.SelectRandomFoundationCapability();
        }

        [When(@"I select an other capability")]
        public void WhenISelectAnOtherCapability()
        {
            addCapabilitiesActions.SelectRandomOtherCapability();
        }

        [Then(@"the capabilities selected counter is '(.*)'")]
        public void ThenTheCapabilitiesSelectedCounterIs(int count)
        {
            addCapabilitiesActions.GetCounter().Should().Be(count);
        }

        [When(@"I select a foundational capability and save its standards")]
        public void WhenISelectAFoundationalCapabilityAndSaveItsStandards()
        {
            standards.AddRange(addCapabilitiesActions.SelectRandomFoundationCapability());
        }

        [When(@"I select an Other capability and save its standards")]
        public void WhenISelectAnOtherCapabilityAndSaveItsStandards()
        {
            standards.AddRange(addCapabilitiesActions.SelectRandomOtherCapability());
        }

        [Then(@"The list of standards contains the standards from the capability")]
        public void ThenTheListOfStandardsContainsTheStandardsFromTheCapability()
        {
            var addedStandards = addCapabilitiesActions.GetAddedCapabilitiesStandards();

            foreach (var standard in standards)
            {
                addedStandards.Should().Contain(standard);
            }
        }

        [Then(@"the capabilities are listed beneath the counter")]
        public void ThenTheCapabilitiesAreListedBeneathTheCounter()
        {
            addCapabilitiesActions.CheckCapabilitesHaveAddedCorrectly();
        }

        [When(@"I click the Save & Complete button")]
        public void ClickSaveAndComplete()
        {
            addCapabilitiesActions.SaveAndComplete();
        }

        [Then(@"an error should be thrown '(.*)'")]
        public void CapabilityError(string error)
        {
            addCapabilitiesActions.GetErrorMessage().Should().Be(error);
        }
    }
}
