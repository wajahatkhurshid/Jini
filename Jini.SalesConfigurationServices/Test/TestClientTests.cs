using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Gyldendal.Jini.SalesConfigurationServices.Test
{
    [TestClass]
    public class TestClientTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Url = "http://dev-testclient-salesconfigurationservices.gyldendal.local/";
            var isbnInput = driver.FindElement(By.Id("isbn"));
            var institutionNumberInput = driver.FindElement(By.Id("institutionNumber"));
            isbnInput.SendKeys("9788702056921");
            institutionNumberInput.SendKeys("335008");
            var checkAvailablityButton = driver.FindElement(By.Id("checkAvailability"));
            var getConfigurationButton = driver.FindElement(By.Id("getConfiguration"));
            checkAvailablityButton.Click();
            driver.SwitchTo().Alert().Accept();
            getConfigurationButton.Click();
            driver.Quit();
        }
    }
}
