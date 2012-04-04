using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using SizSelCsZzz.Extras;
using SizSelCsZzz.Other;

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
            var scriptExecutor = DriverWrapperUnwrapper.GetDriverImplementationOf<IJavaScriptExecutor>(context);

            EnsureSizzleIsLoaded(scriptExecutor);

            IEnumerable<object> scriptResult = null;

            if (context is IWebElement)
            {
                scriptResult = (IEnumerable<object>)scriptExecutor.ExecuteScript("return Sizzle(" + _selector + ", arguments[0])", context);
            }
            else
            {
                scriptResult = (IEnumerable<object>)scriptExecutor.ExecuteScript("return Sizzle(" + _selector + ")");
            }

            var result = new ReadOnlyCollection<IWebElement>(scriptResult.Cast<IWebElement>().ToList());

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
            BigScriptRunner.RunBigScript(driver, ResourceLoader.LoadResourceRelativeToType(typeof (BySizzle), "sizzleSource.sizzle.js"));
        }
    }
}
