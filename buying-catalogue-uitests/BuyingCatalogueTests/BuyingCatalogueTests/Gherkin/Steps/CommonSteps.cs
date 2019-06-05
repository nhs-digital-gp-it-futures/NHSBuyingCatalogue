#pragma warning disable CS1591
using Bogus;
using BuyingCatalogueTests.utils;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    public partial class SpecFlowSteps : UITest
    {
        string errorMessage;
        string solutionName;
        string editView;
        readonly Faker faker;
        List<string> errors;
        string[] nameVersion;

        public SpecFlowSteps()
        {
            login = true;
            faker = new Faker("en_GB");
        }

        [BeforeScenario]
        public void BeforeRun()
        {
            login = true;
            DriverInitialize();
        }

        [AfterScenario]
        public void AfterRun()
        {
            if (login) PostTestReset();
            DriverCleanup();
        }
    }
}
