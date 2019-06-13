#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using BuyingCatalogueTests.utils.EnvData;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;

namespace BuyingCatalogueTests.utils
{
    public abstract class UITest
    {
        internal IWebDriver _driver;
        internal WebDriverWait _wait;

        // Expose the chosen user to the tests
        internal IUser user;

        // Actions setup
        internal PageActions.AuthActions authActions;
        internal PageActions.HomePageActions homePageActions;
        internal PageActions.SolutionsOnboardingActions onboardingActions;
        internal PageActions.SolutionsBasicDetailsActions basicDetailsActions;
        internal PageActions.AddCapabilitiesActions addCapabilitiesActions;
        internal PageActions.CapabilitiesEvidenceActions capEvidenceActions;

        internal PageActions.CommonActions commonActions;

        // Check for if a test needs to authenticate before the body of the test
        internal bool login;

        public UITest(bool login = true)
        {
            this.login = login;
        }

        internal void DriverInitialize()
        {
            var service = new EnvironmentService();

            // Get browser choice from env variable (defaults to Chrome if null or not set)            
            BrowserOptions browser = GetBrowser(service.Browser);
            
            // Get the randomly chosen user
            user = service.user;

            //Initialize driver
            _driver = _driver ?? new DriverConfig(browser).Driver;
            _wait = _wait ?? new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

            // Navigate to the correct URL
            _driver.Url = service.environment.CatalogueUrl;

            // Initialize the actions for the tests
            InitPageActions();

            // Ensure the login page is displayed
            authActions.WaitForLoginPage();            

            // If a test requires authentication, do that now
            if (login)
            {
                authActions.Login(service.user);

                try
                {
                    // Clear the cookie popup if it displays
                    commonActions.ClearCookiePopup();
                }
                catch { }
            }
        }

        internal void PostTestReset()
        {
            // Take screenshot if test failed/errored out
            if (TestContext.CurrentContext.Result.Outcome == ResultState.Failure ||
                TestContext.CurrentContext.Result.Outcome == ResultState.Error)
            {
                // Store screenshot in memory
                Screenshot screenShot = ((ITakesScreenshot)_driver).GetScreenshot();

                // Check if folder exists, create it if not
                var folderPath = Path.Combine(AppContext.BaseDirectory, "Results", "Screenshots", DateTime.Today.ToString("yyyyMMdd"), TestContext.CurrentContext.Test.ClassName.Replace(" ", ""));
                Directory.CreateDirectory(folderPath);

                // Save screenshot to folder
                var filePath = Path.Combine(folderPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                screenShot.SaveAsFile(filePath, ScreenshotImageFormat.Png);

                // Put the path to the file in the console
                TestContext.Out.WriteLine(filePath);
            }

            if (login)
            {
                try
                {
                    commonActions.ClickNHSLogo();
                }
                catch { }
            }
        }

        internal void DriverCleanup()
        {
            // If a test required authentication, logout now
            if (login)
            {
                try
                {
                    authActions.Logout();
                }
                catch { }
            }

            // Quit the browser and exit the webdriver instance
            _driver.Quit();
        }

        private void InitPageActions()
        {
            authActions = new PageActions.AuthActions(_driver);
            homePageActions = new PageActions.HomePageActions(_driver);
            onboardingActions = new PageActions.SolutionsOnboardingActions(_driver);
            basicDetailsActions = new PageActions.SolutionsBasicDetailsActions(_driver);
            addCapabilitiesActions = new PageActions.AddCapabilitiesActions(_driver);
            capEvidenceActions = new PageActions.CapabilitiesEvidenceActions(_driver);

            commonActions = new PageActions.CommonActions(_driver);
        }

        private BrowserOptions GetBrowser(string browser)
        {
            switch (browser)
            {
                case "Edge":
                case "edge":
                    return BrowserOptions.Edge;
                case "Chrome":
                case "chrome":
                case "Google Chrome":
                case "google chrome":
                case "GoogleChrome":
                case "googlechrome":
                    return BrowserOptions.Chrome;
                case "ie":
                case "ie11":
                case "IE":
                case "IE11":
                    return BrowserOptions.IE11;
                default:
                    return BrowserOptions.Chrome;
            }
        }
    }
}