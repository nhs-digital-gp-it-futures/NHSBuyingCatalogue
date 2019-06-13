#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using BuyingCatalogueTests.utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace BuyingCatalogueTests.PageActions
{
    internal class Initialize
    {
        internal IWebDriver _driver;

        internal Objects.AuthObjects authObjects;
        internal Objects.HomePageObjects homePageObjects;
        internal Objects.SolutionsOnboardingObjects onboardingObjects;
        internal Objects.SolutionBasicDetailsObjects basicDetailsObjects;
        internal Objects.AddCapabilitiesObjects addCapabilitiesObjects;
        internal Objects.CapabilitiesEvidenceObjects capEvidenceObjects;
        internal Objects.StandardsEvidenceObjects standardsEvidenceObjects;

        internal Objects.CommonObjects commonObjects;

        internal WebDriverWait _wait;

        public Initialize(IWebDriver driver)
        {
            _driver = driver;
            InitObjects();

            // Max explicit wait times set to 30 seconds
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        }

        private void InitObjects()
        {
            authObjects = new Objects.AuthObjects(_driver);
            homePageObjects = new Objects.HomePageObjects(_driver);
            onboardingObjects = new Objects.SolutionsOnboardingObjects(_driver);
            basicDetailsObjects = new Objects.SolutionBasicDetailsObjects(_driver);
            addCapabilitiesObjects = new Objects.AddCapabilitiesObjects(_driver);
            capEvidenceObjects = new Objects.CapabilitiesEvidenceObjects(_driver);
            standardsEvidenceObjects = new Objects.StandardsEvidenceObjects(_driver);

            commonObjects = new Objects.CommonObjects(_driver);
        }

        // Useful methods to generate By.{Selector} for objects held in the Page Factories (mostly for use with List<> objects)
        internal By GetByOnboardingObject(string obj) => TestUtils.GetByPageFactory(typeof(Objects.SolutionsOnboardingObjects), obj);
        internal By GetByHomeObject(string obj) => TestUtils.GetByPageFactory(typeof(Objects.HomePageObjects), obj);
        internal By GetByBasicDetailsObject(string obj) => TestUtils.GetByPageFactory(typeof(Objects.SolutionBasicDetailsObjects), obj);
        internal By GetByCommonObject(string obj) => TestUtils.GetByPageFactory(typeof(Objects.CommonObjects), obj);
        internal By GetByAuthObject(string obj) => TestUtils.GetByPageFactory(typeof(Objects.AuthObjects), obj);
        internal By GetByAddCapabilitiesObject(string obj) => TestUtils.GetByPageFactory(typeof(Objects.AddCapabilitiesObjects), obj);
        internal By GetByCapEvidenceObject(string obj) => TestUtils.GetByPageFactory(typeof(Objects.CapabilitiesEvidenceObjects), obj);
    }
}