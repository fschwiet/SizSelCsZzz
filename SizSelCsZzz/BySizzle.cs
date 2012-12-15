using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using SizSelCsZzz.Other;

namespace SizSelCsZzz
{
    public class BySizzle : ByExternalScript
    {
        public class BySizzleSelector : ByExternalScript
        {
            public BySizzleSelector(string selector, bool enforceUniqueness)
            {
                this.selector = Newtonsoft.Json.JsonConvert.SerializeObject(selector);
                this.enforceUniqueness = enforceUniqueness;
                javascriptGlobal = "Sizzle";
                resourceLocation = "sizzleSource.sizzle.js";
            }
        }

        public static By CssSelector(string selector)
        {
            return new BySizzleSelector(selector, false);
        }

        public static By Unique(string selector)
        {
            return new BySizzleSelector(selector, true);
        }
    }
}
