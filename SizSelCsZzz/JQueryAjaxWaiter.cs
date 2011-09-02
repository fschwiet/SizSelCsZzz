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
        public static void MonitorJQueryAjax(this IWebDriver webDriver)
        {
            var scriptExecutor = webDriver as IJavaScriptExecutor;

            VerifyJQueryInstalled(scriptExecutor);

            if (!IsFunctionInstalled(scriptExecutor, "SizSelCsZzz_IsRequestPending"))
            {
                var ajaxMonitor = ResourceLoader.LoadResourceRelativeToType(typeof(JQueryAjaxWaiter), "Other.jqueryAjaxMonitor.js");
                scriptExecutor.ExecuteScript(ajaxMonitor);
            }

            if (!IsFunctionInstalled(scriptExecutor, "SizSelCsZzz_IsRequestPending"))
                throw new Exception("Unable to load javascript.");
        }

        public static bool IsAjaxPending(this IWebDriver webDriver)
        {
            var scriptExecutor = webDriver as IJavaScriptExecutor;
            
            VerifyJQueryInstalled(scriptExecutor);

            if (!IsFunctionInstalled(scriptExecutor, "SizSelCsZzz_IsRequestPending"))
            {
                throw new InvalidOperationException("Must call MonitorJQueryAjax before waiting on ajax.");
            }

            return (bool)scriptExecutor.ExecuteScript("return window.SizSelCsZzz_IsRequestPending()");
        }

        public static bool WasMonitoringStarted(this IWebDriver webDriver)
        {
            var scriptExecutor = webDriver as IJavaScriptExecutor;
            return IsFunctionInstalled(scriptExecutor, "SizSelCsZzz_IsRequestPending");
        }

        static void VerifyJQueryInstalled(IJavaScriptExecutor scriptExecutor)
        {
            if (!IsFunctionInstalled(scriptExecutor, "jQuery"))
                throw new JQueryNotInstalledException();
        }

        static bool IsFunctionInstalled(IJavaScriptExecutor scriptExecutor, string functionName)
        {
            return (Boolean)scriptExecutor.ExecuteScript("return typeof(" + functionName + ")=='function'");
        }
    }
}
