using System.ComponentModel;
using System.Text;
using OpenQA.Selenium;
using SizSelCsZzz.Other;

namespace SizSelCsZzz
{
    public class ByJQuery : ByExternalScript
    {
        public class ByJQuerySelector : ByExternalScript
        {
            public ByJQuerySelector(string selector, bool enforceUniqueness)
            {
                this.selector = Newtonsoft.Json.JsonConvert.SerializeObject(selector);
                this.enforceUniqueness = enforceUniqueness;
                javascriptGlobal = "jQuery";
                resourceLocation = "jquerySource.jquery.js";
                resultPrefix = "jQuery.makeArray(";
                resultPosftix = ")";
            }
        }

        public static By CssSelector(string selector)
        {
            return new BySizzle.BySizzleSelector(selector, false);
        }

        public static By Unique(string selector)
        {
            return new BySizzle.BySizzleSelector(selector, true);
        }
    }
}
