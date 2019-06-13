#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using BuyingCatalogueTests.utils;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuyingCatalogueTests.PageActions
{
    internal sealed class SolutionsBasicDetailsActions : Initialize
    {
        public SolutionsBasicDetailsActions(IWebDriver driver) : base(driver)
        {
        }

        public string SaveWithEmptySolutionName()
        {
            SaveSolution();

            _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.SolutionNameError));

            return basicDetailsObjects.SolutionNameError.Text;
        }

        internal void EnterSolutionName(string name)
        {
            basicDetailsObjects.SolutionName.SendKeys(name);
        }

        internal string SaveWithEmptySolutionDescription()
        {
            SaveSolution();

            _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.SolutionDescriptionError));

            return basicDetailsObjects.SolutionDescriptionError.Text;
        }

        internal void SaveSolution()
        {
            basicDetailsObjects.Save.Click();
        }

        internal void EnterSolutionDescription(string description)
        {
            basicDetailsObjects.SolutionDescription.SendKeys(description);
        }

        internal string GetSolutionDetails()
        {
            return basicDetailsObjects.ReadOnlySolutionDetails.Text;
        }

        internal void EnterVersionNumber(string versionNumber)
        {
            basicDetailsObjects.SolutionVersion.SendKeys(versionNumber);
        }

        internal object GetSolutionName()
        {
            return basicDetailsObjects.SolutionName.GetAttribute("value");
        }

        internal List<string> SaveMissingAllContactDetails()
        {
            SaveSolution();

            _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.LeadContactFirstNameError));
            // Get all relevant error messages
            List<string> errorMessages = new List<string>
            {
                basicDetailsObjects.LeadContactFirstNameError.Text,
                basicDetailsObjects.LeadContactLastNameError.Text,
                basicDetailsObjects.LeadContactEmailAddressError.Text,
                basicDetailsObjects.LeadContactPhoneNumberError.Text
            };

            return errorMessages;
        }

        internal void DontSaveChanges()
        {
            var shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            shortWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("unsaved-changes")));

            _driver.FindElement(By.CssSelector("#unsaved-changes a")).Click();
        }

        internal void OpenLeadContactDetailsSection()
        {
            basicDetailsObjects.CollapsibleSections = new Objects.SolutionBasicDetailsObjects(_driver).CollapsibleSections;

            basicDetailsObjects.CollapsibleSections.Single(s => s.Text.Contains("lead contact")).Click();

            _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.LeadContactEmailAddress));
        }

        private void EnterLeadContactFirstName(string name)
        {
            basicDetailsObjects.LeadContactFirstName.SendKeys(name);
        }

        private void EnterLeadContactLastName(string name)
        {
            basicDetailsObjects.LeadContactLastName.SendKeys(name);
        }

        private void EnterLeadContactEmailAddress(string email)
        {
            basicDetailsObjects.LeadContactEmailAddress.SendKeys(email);
        }

        private void EnterLeadContactPhoneNumber(string phone)
        {
            basicDetailsObjects.LeadContactPhoneNumber.SendKeys(phone);
        }

        internal void AddAdditionalContacts(int contacts)
        {
            for (int i = 0; i < contacts; i++)
            {
                basicDetailsObjects.AddAdditionalContact.Click();

                _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id($"solution.contacts[{i + 1}].contactType")));

                ContactDetails details = TestDataGenerator.GenerateTestContact();

                // Due to having to replace part of the ID, this cannot be placed into the object model
                _driver.FindElement(By.Id($"solution.contacts[{i + 1}].contactType")).SendKeys(details.JobTitle);
                _driver.FindElement(By.Id($"solution.contacts[{i + 1}].firstName")).SendKeys(details.FirstName);
                _driver.FindElement(By.Id($"solution.contacts[{i + 1}].lastName")).SendKeys(details.LastName);
                _driver.FindElement(By.Id($"solution.contacts[{i + 1}].emailAddress")).SendKeys(details.EmailAddress);
                _driver.FindElement(By.Id($"solution.contacts[{i + 1}].phoneNumber")).SendKeys(details.PhoneNumber);
            }
        }

        internal void SaveAndContinue(string pageTitle)
        {
            basicDetailsObjects.SaveAndSubmit.Click();

            _wait.Until(ExpectedConditions.ElementToBeClickable(commonObjects.PageHeader));

            commonObjects.PageHeader.Text.Should().Be(pageTitle);
        }

        internal string GetSolutionNameError()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.SolutionNameError));

            return basicDetailsObjects.SolutionNameError.Text;
        }

        internal string GetSolutionDescriptionError()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(basicDetailsObjects.SolutionDescriptionError));

            return basicDetailsObjects.SolutionDescriptionError.Text;
        }

        internal void EnterLeadContactDetails(ContactDetails details)
        {
            EnterLeadContactFirstName(details.FirstName);
            EnterLeadContactLastName(details.LastName);
            EnterLeadContactEmailAddress(details.EmailAddress);
            EnterLeadContactPhoneNumber(details.PhoneNumber);
        }
    }
}