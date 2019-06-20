using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;

namespace BuyingCatalogueTests.utils
{
    internal enum BrowserOptions
    {
        Chrome, Edge, IE11
    }

    internal sealed class DriverConfig
    {
        internal readonly IWebDriver Driver;

        public DriverConfig(BrowserOptions browser)
        {
            Driver = InitDriver(browser);
        }

        private IWebDriver InitDriver(BrowserOptions browser)
        {
            IWebDriver driver;

            switch (browser)
            {
                case BrowserOptions.Chrome:
                    driver = SetupChromeDriver();
                    break;
                case BrowserOptions.Edge:
                    driver = SetupEdgeDriver();
                    break;
                case BrowserOptions.IE11:
                    driver = SetupIEDriver();
                    break;
                default:
                    throw new WebDriverException("Unrecognised browser selected");
            }

            return driver;
        }

        private IWebDriver SetupChromeDriver()
        {
            ChromeOptions options = new ChromeOptions();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                options.AddArguments("--start-maximized", "--auto-open-devtools-for-tabs");
            }
            else
            {
                options.AddArguments("--headless", "--disable-gpu", "window-size=1920,1080");
            }

            return new ChromeDriver(options);
        }

        private IWebDriver SetupEdgeDriver()
        {
            var options = new EdgeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal,
                UseInPrivateBrowsing = true,
                UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore
            };

            return new EdgeDriver(options);
        }

        private IWebDriver SetupIEDriver()
        {
            InternetExplorerOptions options = new InternetExplorerOptions
            {
                IgnoreZoomLevel = true
            };

            IWebDriver driver = new InternetExplorerDriver(options);

            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
