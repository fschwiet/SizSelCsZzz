using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz
{
    public class BySizzle : By
    {
        public static By CssSelector(string selector)
        {
            return new BySizzle(selector);
        }

        private readonly string _selector;

        public BySizzle(string selector)
        {
            _selector = Newtonsoft.Json.JsonConvert.SerializeObject(selector);
        }

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            var javascriptExpression = "return Sizzle(" + _selector + ")";

            var scriptExecutor = context as IJavaScriptExecutor;

            if (scriptExecutor == null && context is IWrapsDriver)
            {
                scriptExecutor = (context as IWrapsDriver).WrappedDriver as IJavaScriptExecutor;
            }

            var result = new ReadOnlyCollection<IWebElement>(GetMatches(scriptExecutor, javascriptExpression).Cast<IWebElement>().ToList());

            return result;
        }

        public override IWebElement FindElement(ISearchContext context)
        {
            var found = FindElements(context);

            if (found.Count == 0)
            {
                throw new NoSuchElementException("Could not find element matching css selector '" + _selector + "'.");
            }

            return found.First();
        }

        private static IEnumerable<object> GetMatches(IJavaScriptExecutor scriptExecutor, string javascriptExpression)
        {
            EnsureSizzleIsLoaded(scriptExecutor);

            return ((IEnumerable<object>) scriptExecutor.ExecuteScript(javascriptExpression));
        }

        static void EnsureSizzleIsLoaded(IJavaScriptExecutor scriptExecutor)
        {
            if (!SizzleLoaded(scriptExecutor))
            {
                InjectSizzle(scriptExecutor);
            }
        }

        static bool SizzleLoaded(IJavaScriptExecutor scriptExecutor)
        {
            bool loaded;
            try
            {
                loaded = (Boolean)scriptExecutor.ExecuteScript("return Sizzle()!=null");
            }
            catch (Exception e)
            {
                loaded = false;
            }
            return loaded;
        }

        static void InjectSizzle(IJavaScriptExecutor driver)
        {
            var sizzleScript = ResourceLoader.LoadResourceRelativeToType(typeof (BySizzle), "sizzleSource.sizzle.js");

            driver.ExecuteScript(
                "var scriptSource = " +
                Newtonsoft.Json.JsonConvert.SerializeObject(sizzleScript) + ";"
                + @"
var newScript = document.createElement('script');
newScript.type = 'text/javascript';

if (typeof (newScript.appendChild) == 'function') {
    var scriptContent = document.createTextNode(scriptSource);
    newScript.appendChild(scriptContent);
} else {
    newScript.text = scriptSource;
}

var headID = document.getElementsByTagName('head')[0];
headID.appendChild(newScript);
");
        }
    }
}
