using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuyingCatalogueTests.PageActions
{
    internal sealed class StandardsEvidenceActions : Initialize
    {
        public StandardsEvidenceActions(IWebDriver driver) : base(driver)
        {
        }

        internal string GetPageHeaderText()
        {
            return standardsEvidenceObjects.PageHeader.Text;
        }

        internal IList<string> GetAssociatedStandards()
        {
            standardsEvidenceObjects.AssociatedStandards = new Objects.StandardsEvidenceObjects(_driver).AssociatedStandards;
            return standardsEvidenceObjects.AssociatedStandards.Select(s => s.FindElement(By.ClassName("name")).Text).ToList();
        }

        internal IList<string> GetOverarchingStandards()
        {
            standardsEvidenceObjects.OverarchingStandards = new Objects.StandardsEvidenceObjects(_driver).OverarchingStandards;
            return standardsEvidenceObjects.OverarchingStandards.Select(s => s.FindElement(By.ClassName("name")).Text).ToList();
        }

        internal double GetStandardsTables()
        {
            standardsEvidenceObjects.StandardsTables = new Objects.StandardsEvidenceObjects(_driver).StandardsTables;
            return standardsEvidenceObjects.StandardsTables.Count;
        }

        internal void RevealHelpContents()
        {
            standardsEvidenceObjects.HelpRevealTitle.Click();
        }

        internal string GetHelpContents()
        {
            return standardsEvidenceObjects.HelpRevealContent.Text;
        }
    }
}