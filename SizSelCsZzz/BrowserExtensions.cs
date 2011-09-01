using System;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SizSelCsZzz
{
    public static class BrowserExtensions
    {
        public static int MaxWaitMS = 5000;

        public static IWebElement WaitForElement(this IWebDriver browser, By condition, int? maxWaitMS)
        {
            maxWaitMS = maxWaitMS ?? MaxWaitMS;

            return new WebDriverWait(browser, TimeSpan.FromMilliseconds(MaxWaitMS)).Until(driver => driver.FindElement(condition));
        }

        public static string GetBodyText(this IWebDriver browser)
        {
            if (browser.FindElements(By.TagName("body")).Count() == 0)
            {
                return "";
            }

            return browser.FindElement(By.TagName("body")).Text;
        }

        public static bool ContainsText(this IWebDriver browser, string text)
        {
            return browser.GetBodyText().Contains(text);
        }

        public static int CountElementsMatching(this IWebDriver browser, string cssSelector)
        {
            int sum = 0;

            foreach(var selector in cssSelector.Split(','))
            {
                sum += browser.FindElements(By.CssSelector(selector)).Count();
            }

            return sum;
        }
    }
}
