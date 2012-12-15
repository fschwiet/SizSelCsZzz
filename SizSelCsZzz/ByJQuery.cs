using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using SizSelCsZzz.Other;

namespace SizSelCsZzz
{
    public class ByJQuery : ByExternalScript
    {
        public class ByJQuerySelector : ByExternalScript
        {
            public ByJQuerySelector(string selector)
            {
                this.selector = Newtonsoft.Json.JsonConvert.SerializeObject(selector);
                javascriptGlobal = "jQuery";
                resourceLocation = "jquerySource.jquery.js";
                resultPrefix = "jQuery.makeArray(";
                resultPosftix = ")";
            }
        }

        public static By CssSelector(string selector)
        {
            return new BySizzle.BySizzleSelector(selector);
        }
    }
}
