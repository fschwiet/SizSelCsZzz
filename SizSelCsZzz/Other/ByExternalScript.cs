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
        protected string _resultPrefix = "";
        protected string _resultPosftix = "";

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            var scriptExecutor = DriverWrapperUnwrapper.GetDriverImplementationOf<IJavaScriptExecutor>(context);

            EnsureScriptIsLoaded(scriptExecutor);

            IEnumerable<object> scriptResult = null;

            object executeScript;

            if (context is IWebElement)
            {
                executeScript = scriptExecutor.ExecuteScript("return " + _resultPrefix + _javascriptGlobal + "(" + _selector + ", arguments[0])" + _resultPosftix, context);
            }
            else
            {
                executeScript = scriptExecutor.ExecuteScript("return " + _resultPrefix + _javascriptGlobal + "(" + _selector + ")" + _resultPosftix);
            }

            scriptResult = (IEnumerable<object>)executeScript;

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
                BigScriptRunner.RunBigScript(scriptExecutor, ResourceLoader.LoadResourceRelativeToType(typeof(ByJQuery), _resourceLocation));
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