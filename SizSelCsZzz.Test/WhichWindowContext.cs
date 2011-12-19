using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace SizSelCsZzz.Test
{
    public class WhichWindowContext
    {
        private readonly IWebDriver _browser;
        private readonly List<string> _existingWindows;
        private string _originalWindow;

        public WhichWindowContext(IWebDriver browser)
        {
            _browser = browser;

            _existingWindows = new List<string>(browser.WindowHandles);
            _originalWindow = browser.CurrentWindowHandle;
        }

        public string GetNewWindowName()
        {
            var currentWindows = _browser.WindowHandles;

            var newWindows = currentWindows.Where(w => !_existingWindows.Contains(w));

            var resultCount = newWindows.Count();

            if (resultCount == 0)
                return null;
            else if (resultCount == 1)
                return newWindows.Single();
            else
                throw new Exception("multiple new windows found");
        }

        public string GetOriginalWindowName()
        {
            return _originalWindow;
        }
    }
}