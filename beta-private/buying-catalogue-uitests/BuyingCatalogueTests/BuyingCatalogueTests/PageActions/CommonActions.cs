#pragma warning disable 1591

using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace BuyingCatalogueTests.PageActions
{
    internal sealed class CommonActions : Initialize
    {
        public CommonActions(IWebDriver driver) : base(driver)
        {
        }

        internal void ClickNHSLogo()
        {
            commonObjects.NHSLogo.Click();

            try
            {
                new SolutionsBasicDetailsActions(_driver).DontSaveChanges();
            }
            catch { }

            _wait.Until(ExpectedConditions.ElementToBeClickable(homePageObjects.AddSolution));
        }

        internal void ClearCookiePopup()
        {
            // Tries to clear the cookie popup 3 times before giving up and throwing an error
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    _driver.FindElement(By.CssSelector("a.cc-allow")).Click();
                    _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("a.cc-allow")));
                    break;
                }
                catch
                {
                    if (i == 2) throw new WebDriverTimeoutException("Could not clear the cookie popup");
                }
            }
        }

        internal void SaveAndSubmit()
        {
            commonObjects.SaveAndSubmit.Click();
        }

        internal void WaitForHeader(string headerText)
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(commonObjects.PageHeader));

            commonObjects.PageHeader.Text.Should().Be(headerText);
        }
    }
}