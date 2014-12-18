using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.Specs {
    public class CommandPage {
        [FindsBy(How = How.Name, Using = "File")]
        private IWebElement cmd;

        [FindsBy(How = How.Name, Using = "Arguments")]
        private IWebElement args;

        [FindsBy(How = How.Id, Using = "submit")]
        private IWebElement submit;

        [FindsBy(How = How.Id, Using = "Output")]
        private IWebElement Output;

        private static IWebDriver driver;

        public static CommandPage NavigateTo(IWebDriver webDriver) {
            driver = webDriver;
            driver.Navigate().GoToUrl("http://localhost:7777/command");
            var page = new CommandPage();
            PageFactory.InitElements(driver, page);
            return page;
        }

        public void echo(string cmds) {
            cmd.SendKeys(cmds);
            args.SendKeys("");
            submit.Click();

            var page = new CommandPage();
            PageFactory.InitElements(driver, page);
        }

        public bool IsThere( string exe ) {
            var output = Output.Text;
            return Output.Text.Contains( exe );
        }
    }
}
