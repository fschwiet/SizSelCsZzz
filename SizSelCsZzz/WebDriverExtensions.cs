using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SizSelCsZzz
{
    public static class WebDriverExtensions
    {
        public static int MaxWaitMS = 5000;
        public static int WaitIntervalMS = 100;

        public static IWebElement WaitForElement(this ISearchContext node, By condition)
        {
            return node.WaitForElementEx(condition);
        }

        public static IWebElement WaitForElementEx(this ISearchContext node, By condition, int? maxWaitMS = null, int? waitIntervalMS = null)
        {
            maxWaitMS = maxWaitMS ?? MaxWaitMS;
            waitIntervalMS = waitIntervalMS ?? WaitIntervalMS;

            DateTime finishTime = DateTime.UtcNow.AddMilliseconds(maxWaitMS.Value);

            do
            {
                try
                {
                    return node.FindElement(condition);
                }
                catch (Exception)
                {
                    if (DateTime.UtcNow > finishTime)
                    {
                        throw;
                    }
                }

                Thread.Sleep(waitIntervalMS.Value);
            } while (true);
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
            return browser.FindElements(BySizzle.CssSelector(cssSelector)).Count();
        }
    }
}
