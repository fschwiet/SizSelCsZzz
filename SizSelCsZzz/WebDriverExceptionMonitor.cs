using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace SizSelCsZzz
{
    public class WebDriverExceptionMonitor
    {
        public WebDriverExceptionMonitor()
        {
        }

        public WebDriverExceptionMonitor Monitor(IWebDriver browser)
        {
            return this;
        }

        public IEnumerable<string> GetJavascriptExceptions()
        {
            return new string[0];
        }
    }
}