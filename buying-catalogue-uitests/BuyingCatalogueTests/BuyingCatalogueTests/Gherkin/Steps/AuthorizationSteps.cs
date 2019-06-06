#pragma warning disable CS1591
using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    public sealed partial class SpecFlowSteps
    {
        [Given(@"I enter my username and password and click login")]
        public void GivenIEnterMyUsernameAndPasswordAndClickLogin()
        {
            authActions.Login(user);
        }

        [When(@"I click Logout")]
        public void WhenIClickLogout()
        {
            authActions.Logout();
        }
    }
}
