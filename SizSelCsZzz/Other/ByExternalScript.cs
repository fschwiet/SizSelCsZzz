using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Other
{
    public class ByExternalScript : By
    {
        protected string _selector;
        protected string _javascriptGlobal;
        protected string _resourceLocation;

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            var scriptExecutor = DriverWrapperUnwrapper.GetDriverImplementationOf<IJavaScriptExecutor>(context);

            EnsureScriptIsLoaded(scriptExecutor);

            IEnumerable<object> scriptResult = null;

            if (context is IWebElement)
            {
                scriptResult = (IEnumerable<object>)scriptExecutor.ExecuteScript("return " + _javascriptGlobal + "(" + _selector + ", arguments[0])", context);
            }
            else
            {
                scriptResult = (IEnumerable<object>)scriptExecutor.ExecuteScript("return " + _javascriptGlobal + "(" + _selector + ")");
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

        void EnsureScriptIsLoaded(IJavaScriptExecutor scriptExecutor)
        {
            if (!IsScriptLoaded(scriptExecutor))
            {
                BigScriptRunner.RunBigScript(scriptExecutor, ResourceLoader.LoadResourceRelativeToType(typeof(ByExternalScript), _resourceLocation));
            }
        }

        bool IsScriptLoaded(IJavaScriptExecutor scriptExecutor)
        {
            bool loaded;
            try
            {
                loaded = (Boolean)scriptExecutor.ExecuteScript("return typeof(" + _javascriptGlobal + ")==='function'");
            }
            catch (Exception e)
            {
                loaded = false;
            }
            return loaded;
        }
    }
}