using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using TechTalk.SpecFlow;

namespace Antd.Specs {
    [Binding]
    public class CommandSteps {

        private static IWebDriver driver;
        private CommandPage page;

        [BeforeScenario()]
        public void Setup() {
            driver = new ChromeDriver();
        }

        [AfterScenario()]
        public void TearDown() {
            driver.Quit();
        }

        [Given(@"I go to page command")]
        public void GivenIGoToPageCommand() {
            page = CommandPage.NavigateTo(driver);
        }

        [When(@"I check ""(.*)"" command result")]
        public void WhenICheckCommandResult(string p0) {
            page.echo(p0);
        }

        [Then(@"The result contains ""(.*)"" string")]
        public void ThenTheResultContainsString(string p0) {
            Assert.IsTrue( page.IsThere(p0) );
        }
    }
}
