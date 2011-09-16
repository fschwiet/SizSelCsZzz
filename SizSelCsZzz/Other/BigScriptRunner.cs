using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace SizSelCsZzz.Other
{
    //  I don't really recall why I don't just run the script.
    public class BigScriptRunner
    {
        public static void RunBigScript(IJavaScriptExecutor driver, string script)
        {
            driver.ExecuteScript(
                "var scriptSource = " +
                JsonConvert.SerializeObject(script) + ";"
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
