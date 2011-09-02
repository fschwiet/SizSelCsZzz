using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;
using SizSelCsZzz.Other;

namespace SizSelCsZzz
{
    public static class JQueryAjaxWaiter
    {
        public static bool IsAjaxPending(this IWebDriver webDriver)
        {
            var scriptExecutor = webDriver as IJavaScriptExecutor;
            
            if (!IsJQueryInstalled(scriptExecutor))
                throw new JQueryNotInstalledException();

            return false;
        }

        static bool IsJQueryInstalled(IJavaScriptExecutor scriptExecutor)
        {
            return (Boolean)scriptExecutor.ExecuteScript("return typeof(jQuery)=='function'");
        }
    }
}
