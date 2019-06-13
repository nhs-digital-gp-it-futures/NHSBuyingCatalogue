#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using BuyingCatalogueTests.utils.EnvData;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using BuyingCatalogueTests.utils;

namespace BuyingCatalogueTests.PageActions
{
    internal sealed class AuthActions : Initialize
    {
        int retries = 0;

        public AuthActions(IWebDriver driver) : base(driver)
        {
        }

        public void WaitForLoginPage()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(authObjects.LogInOutLink));
        }

        public void Login(IUser user)
        {
            if (retries == 0)
            {
                authObjects.LogInOutLink.Click();
            }

            //_wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("auth0-lock-header-logo")));

            try
            {
                _wait.Until(ExpectedConditions.ElementToBeClickable(authObjects.EmailInput));
            }
            catch
            {
                if (_driver.GetType().ToString().Contains("EdgeDriver") && _driver.FindElements(By.CssSelector("input[type=email]")).Count == 0)
                {
                    authObjects.NotYourAccountLink.Click();
                    _wait.Until(ExpectedConditions.ElementToBeClickable(authObjects.EmailInput));
                }
            }            

            authObjects.EmailInput.SendKeys(user.UserName);
            authObjects.PasswordInput.SendKeys(user.Password);

            authObjects.LoginButton.Click();

            try
            {
                _wait.Until(s => _driver.Title == "NHS Digital Buying Catalogue");
                _wait.Until(ExpectedConditions.ElementToBeClickable(homePageObjects.AddSolution));
                retries = 0;
            }
            catch
            {
                if (retries == 2)
                {
                    throw new TimeoutException("Exceeded maximum retries");
                }

                retries++;
                authObjects.NotYourAccountLink.Click();
                Login(user);
            }
        }

        public void Logout()
        {
            authObjects.LogInOutLink.Click();

            if (_driver.Url.Contains("localhost"))
            {
                _driver.FindElement(By.CssSelector("body > div > button:nth-of-type(1)")).Click();
            }

            try
            {
                new SolutionsBasicDetailsActions(_driver).DontSaveChanges();
            }
            catch { }

            _wait.Until(s => authObjects.LogInOutLink.Text == "Log in");
        }

        public string GetLoginOutLinkText()
        {
            return authObjects.LogInOutLink.Text;
        }

        internal void ClickNotAccount()
        {
            authObjects.NotYourAccountLink.Click();
            _wait.Until(ExpectedConditions.ElementToBeClickable(authObjects.EmailInput));
        }
    }
}