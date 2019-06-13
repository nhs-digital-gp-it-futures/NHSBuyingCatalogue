using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System;
using System.Linq;

namespace BuyingCatalogueTests.utils
{
    internal static class TestUtils
    {
        internal static By GetByPageFactory(Type type, string property)
        {
            // Select a property and grab the [FindsBy()] How and Value
            FindsByAttribute attr = (FindsByAttribute)type
                .GetProperty(property)
                .GetCustomAttributes(false)
                .First(s => s.GetType() == typeof(FindsByAttribute));

            // Return a converted By value for the relevant selector, i.e. [FindsBy(How=How.Id, Using = "Bob")] => By.Id("Bob")
            switch (attr.How)
            {
                case How.Id:
                    return By.Id(attr.Using);
                case How.CssSelector:
                    return By.CssSelector(attr.Using);
                case How.ClassName:
                    return By.ClassName(attr.Using);
                case How.LinkText:
                    return By.LinkText(attr.Using);
                case How.Name:
                    return By.Name(attr.Using);
                case How.PartialLinkText:
                    return By.PartialLinkText(attr.Using);
                case How.TagName:
                    return By.TagName(attr.Using);
                case How.XPath:
                    return By.XPath(attr.Using);
                default:
                    throw new Exception("Selector method not recognised. Did you mean something else?");
            }
        }
    }
}
