using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
namespace Web_automation
{
    internal class Class1
    {
        public static void Main()
        {
            IWebDriver driver = new FirefoxDriver();

            

            driver.Navigate().GoToUrl("https://minecraftart.netlify.app/editor");

            var title = driver.Title;

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

           

            driver.FindElement(By.Name("main-image")).SendKeys("C:\\Users\\Letha\\OneDrive\\Desktop\\PNG_transparency_demonstration_1.png");

            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("settings-submit")));

            driver.FindElement(By.Id("settings-input-width")).Clear();
            driver.FindElement(By.Id("settings-input-width")).SendKeys("256");

            driver.FindElement(By.Id("settings-input-height")).Clear();
            driver.FindElement(By.Id("settings-input-height")).SendKeys("256");

            driver.FindElement(By.Id("settings-submit")).Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("topbar")));

            driver.FindElement(By.XPath("/ html / body / div[3] / section[3] / div[2] / div[2]")).Click();

            var filename = driver.FindElement(By.Id("editor-save-input"));
            filename.Clear();
            filename.SendKeys("Minecraft_Structure_Demo");

            driver.FindElement(By.XPath("//*[@id=\"editor-save-btn\"]")).Click();

            driver.Quit();


        }
    }
}
