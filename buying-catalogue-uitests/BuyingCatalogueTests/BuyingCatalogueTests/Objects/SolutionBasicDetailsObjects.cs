#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Collections.Generic;

namespace BuyingCatalogueTests.Objects
{
    internal class SolutionBasicDetailsObjects : Initialization
    {
        public SolutionBasicDetailsObjects(IWebDriver driver) : base(driver)
        {
        }

        #region ErrorMessages

        [FindsBy(How = How.Id, Using = "error-solution.name")]
        public IWebElement SolutionNameError { get; set; }

        [FindsBy(How = How.Id, Using = "error-solution.description")]
        public IWebElement SolutionDescriptionError { get; set; }

        [FindsBy(How = How.Id, Using = "error-solution.contacts[0].firstName")]
        public IWebElement LeadContactFirstNameError { get; set; }

        [FindsBy(How = How.Id, Using = "error-solution.contacts[0].lastName")]
        public IWebElement LeadContactLastNameError { get; set; }

        [FindsBy(How = How.Id, Using = "error-solution.contacts[0].emailAddress")]
        public IWebElement LeadContactEmailAddressError { get; set; }

        [FindsBy(How = How.Id, Using = "error-solution.contacts[0].phoneNumber")]
        public IWebElement LeadContactPhoneNumberError { get; set; }

        #endregion

        #region SolutionDetails

        [FindsBy(How = How.Id, Using = "solution.name")]
        public IWebElement SolutionName { get; set; }

        [FindsBy(How = How.CssSelector, Using = "input[value=Save]")]
        public IWebElement Save { get; set; }

        [FindsBy(How = How.Id, Using = "solution.description")]
        public IWebElement SolutionDescription { get; set; }

        [FindsBy(How = How.Id, Using = "solution.version")]
        public IWebElement SolutionVersion { get; set; }

        #endregion

        #region ContactDetails

        [FindsBy(How = How.Id, Using = "solution.contacts[0].firstName")]
        public IWebElement LeadContactFirstName { get; set; }

        [FindsBy(How = How.Id, Using = "solution.contacts[0].lastName")]
        public IWebElement LeadContactLastName { get; set; }

        [FindsBy(How = How.Id, Using = "solution.contacts[0].emailAddress")]
        public IWebElement LeadContactEmailAddress { get; set; }

        [FindsBy(How = How.Id, Using = "solution.contacts[0].phoneNumber")]
        public IWebElement LeadContactPhoneNumber { get; set; }

        [FindsBy(How = How.Id, Using = "add-contact-button")]
        public IWebElement AddAdditionalContact { get; set; }

        #endregion

        [FindsBy(How = How.CssSelector, Using = "fieldset.collapsible legend")]
        public IList<IWebElement> CollapsibleSections { get; set; }

        [FindsBy(How = How.Id, Using = "summary-details")]
        public IWebElement ReadOnlySolutionDetails { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#registration-form input.button.primary")]
        internal IWebElement SaveAndSubmit { get; set; }
    }
}
