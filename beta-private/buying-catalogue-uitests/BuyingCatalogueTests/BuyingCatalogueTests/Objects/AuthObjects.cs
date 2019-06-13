#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace BuyingCatalogueTests.Objects
{
    internal sealed class AuthObjects : Initialization
    {
        public AuthObjects(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.CssSelector, Using = "div.auth a")]
        internal IWebElement LogInOutLink { get; set; }

        [FindsBy(How = How.CssSelector, Using = "input[type=email]")]
        internal IWebElement EmailInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = "input[type=password]")]
        internal IWebElement PasswordInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = "button[type=submit]")]
        internal IWebElement LoginButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "auth0-lock-alternative-link")]
        internal IWebElement NotYourAccountLink { get; set; }
    }
}
