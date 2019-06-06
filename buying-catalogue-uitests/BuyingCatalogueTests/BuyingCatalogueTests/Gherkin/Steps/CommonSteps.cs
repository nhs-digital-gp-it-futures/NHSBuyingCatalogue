#pragma warning disable CS1591
using Bogus;
using BuyingCatalogueTests.utils;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace BuyingCatalogueTests.Gherkin.Steps
{
    public sealed partial class SpecFlowSteps : UITest
    {
        string errorMessage;
        string solutionName;
        string editView;
        readonly Faker faker = new Faker("en_GB");
        List<string> errors;
        string[] nameVersion;

        public SpecFlowSteps()
        {
            login = true;
        }

        [BeforeScenario]
        public void BeforeRun()
        {
            DriverInitialize();
        }

        [AfterScenario]
        public void AfterRun()
        {
            PostTestReset();
            DriverCleanup();
        }
    }
}
