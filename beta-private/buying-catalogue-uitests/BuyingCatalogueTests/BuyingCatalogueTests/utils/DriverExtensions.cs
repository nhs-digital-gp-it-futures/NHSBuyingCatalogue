using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace BuyingCatalogueTests.utils
{
    internal static class DriverExtensions
    {
        internal static void DragAndDrop(this IWebDriver driver, IWebElement source, IWebElement target)
        {
            Actions builder = new Actions(driver);
            builder.DragAndDrop(source, target);
            builder.Build().Perform();
        }

        internal static void DeleteAllCookiesAndHardRefresh(this IWebDriver driver)
        {
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Navigate().Refresh();
        }
    }
}
