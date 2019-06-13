#pragma warning disable CS1591
using NUnit.Framework;

namespace BuyingCatalogueTests.utils
{
    public abstract class E2ETest : UITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            DriverInitialize();
        }

        [TearDown]
        public void TestReset()
        {
            PostTestReset();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DriverCleanup();
        }
    }
}
