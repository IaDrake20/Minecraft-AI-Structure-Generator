using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

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

            var imagePicker = driver.FindElement(By.Name("file-label"));
            imagePicker.Click();

           // var textBox = driver.FindElement(By.Name("my-text"));
           //var submitButton = driver.FindElement(By.TagName("button"));

           // textBox.SendKeys("Selenium");
           // submitButton.Click();

           // var message = driver.FindElement(By.Id("message"));
           // var value = message.Text;

            driver.Quit();

            Console.WriteLine("TITLE: "+title);
        }
    }
}
