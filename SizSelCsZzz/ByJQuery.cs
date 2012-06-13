using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using SizSelCsZzz.Other;

namespace SizSelCsZzz
{
    public class ByJQuery : ByExternalScript
    {
        public static By CssSelector(string selector)
        {
            return new ByJQuery()
            {
                _selector = Newtonsoft.Json.JsonConvert.SerializeObject(selector),
                _javascriptGlobal = "jQuery",
                _resourceLocation = "jquerySource.jquery.js",
                _resultPrefix = "jQuery.makeArray(",
                _resultPosftix = ")"
            };
        }
    }
}
