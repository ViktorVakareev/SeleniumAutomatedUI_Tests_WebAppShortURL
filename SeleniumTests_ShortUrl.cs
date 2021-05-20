using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace SeleniumAutomatedUI_Tests_WebAppShortURL
{
    public class SeleniumUITests
    {
        IWebDriver driver;
        private const string shortUrlHome = "https://shorturl.viktorvakareev.repl.co/";

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            driver.Navigate().GoToUrl(shortUrlHome);
        }

        // •	Open the [Short URLs] page and ensure it holds a table with short URLs.
        [Test]
        public void TestShortUrlPageHoldsTableOfShortUrls()
        {
            var shortUrlsTab = driver.FindElement(By.CssSelector("body > header > a:nth-child(3)"));
            shortUrlsTab.Click();
            var shortUrlTableColumn = driver.FindElement(By.CssSelector("body > main > table > tbody > tr:nth-child(1) > th:nth-child(2)"));
            var shortUrlTableFirstRow = driver.FindElement(By.CssSelector("body > main > table > tbody > tr > td > a.shorturl"));

            Assert.AreEqual("Short URL", shortUrlTableColumn.Text);
            Assert.AreEqual("http://shorturl.viktorvakareev.repl.co/go/nak", shortUrlTableFirstRow.Text);
        }

        // •	Create new short URL from the[Add URL] (valid data & invalid data).

        [Test]
        public void TestCreateNewUrl_ValidData()
        {
            var homeTab = driver.FindElement(By.CssSelector("body > header > a"));
            homeTab.Click();

            int currentUrlCount = int.Parse(driver.FindElement(By.CssSelector("main > ul > li > b")).Text);

            //var addUrlTab = driver.FindElement(By.CssSelector("body > header > a:nth-child(5)"));
            //addUrlTab.Click();
            driver.Navigate().GoToUrl("https://shorturl.viktorvakareev.repl.co/add-url");
            Thread.Sleep(3000);

            var newUrlTextBox = driver.FindElement(By.CssSelector("form > div > input#url.url"));
            newUrlTextBox.Clear();
            string newCreatedUrl = "https://stackoverflow.com/" + DateTime.Now.Ticks;
            newUrlTextBox.SendKeys(newCreatedUrl);

            var shortCodeTextBox = driver.FindElement(By.CssSelector("form > div > input#code.url"));
            shortCodeTextBox.Clear();
            string newCreatedUrlCode = "sof" + DateTime.Now.Ticks;
            shortCodeTextBox.SendKeys(newCreatedUrlCode);

            var buttonCreateNewShortUrl = driver.FindElement(By.CssSelector("main > form > button"));
            buttonCreateNewShortUrl.Click();

            driver.Navigate().GoToUrl(shortUrlHome);
            int addedUrlCount = int.Parse(driver.FindElement(By.CssSelector("main > ul > li > b")).Text);
            Assert.AreEqual(1, addedUrlCount - currentUrlCount);
        }

        [Test]
        public void TestCreateNewUrl_InvalidData()
        {

            var addUrlTab = driver.FindElement(By.CssSelector("body > header > a:nth-child(5)"));
            addUrlTab.Click();

            Thread.Sleep(2000);
            var newUrlTextBox = driver.FindElement(By.CssSelector("form > div > input#url.url"));
            newUrlTextBox.Clear();
            string newCreatedUrl = "stackoverflow.com/" + DateTime.Now.Ticks;
            newUrlTextBox.SendKeys(newCreatedUrl);

            var shortCodeTextBox = driver.FindElement(By.CssSelector("form > div > input#code.url"));
            shortCodeTextBox.Clear();
            string newCreatedUrlCode = "sof" + DateTime.Now.Ticks;
            shortCodeTextBox.SendKeys(newCreatedUrlCode);

            var buttonCreateNewShortUrl = driver.FindElement(By.CssSelector("main > form > button"));
            buttonCreateNewShortUrl.Click();

            Thread.Sleep(2000);
            var errorMessage = driver.FindElement(By.CssSelector("body > div.err"));

            Assert.AreEqual("Invalid URL!", errorMessage.Text);
        }

        // •	Visit existing short URL and ensure that the visitors counter increases.
        [Test]
        public void TestVisitExsistingShortUrl_CheckCounterIncreases_ValidData()
        {
            var shortUrlsTab = driver.FindElement(By.CssSelector("body > header > a:nth-child(3)"));
            shortUrlsTab.Click();
            Thread.Sleep(3000);

            var currentVisits = driver.FindElement(By.CssSelector("table > tbody > tr:nth-child(4) > td:nth-child(4)"));
            int currentVisitsCount = int.Parse(currentVisits.Text);

            var nodeShortUrl = driver.FindElement(By.CssSelector("table > tbody > tr:nth-child(4) > td:nth-child(2) > a"));
            nodeShortUrl.Click();

            driver.Navigate().GoToUrl("https://shorturl.viktorvakareev.repl.co/urls");
            var addedVisits = driver.FindElement(By.CssSelector("table > tbody > tr:nth-child(4) > td:nth-child(4)"));
            int addedVisitsCount = int.Parse(addedVisits.Text);

            Assert.AreEqual(1, addedVisitsCount - currentVisitsCount);
        }
        [Test]
        public void TestVisitNonExsistingShortUrl_ErrorMessage()
        {
            var shortUrlsTab = driver.FindElement(By.CssSelector("body > header > a:nth-child(3)"));
            shortUrlsTab.Click();
            Thread.Sleep(3000);
            string nonExsistingUrl = "non - existing - shorturl";
            driver.Navigate().GoToUrl("http://shorturl.viktorvakareev.repl.co/go/" + nonExsistingUrl);

            var invalidMessage1 = driver.FindElement(By.CssSelector("body > div.err"));
            var invalidMessage2 = driver.FindElement(By.CssSelector("body > main > h1"));
            var invalidShortUrl = driver.FindElement(By.CssSelector("body > main > p"));

            Assert.AreEqual("Invalid short code!", invalidMessage1.Text);
            Assert.AreEqual("Cannot navigate to given short URL", invalidMessage2.Text);
            Assert.AreEqual(("Invalid short URL code: " + nonExsistingUrl), invalidShortUrl.Text);
        }
        [OneTimeTearDown]
        public void Shutdown()
        {
            driver.Quit();
        }

    }
}