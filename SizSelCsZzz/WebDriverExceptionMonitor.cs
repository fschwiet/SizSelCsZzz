using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using SizSelCsZzz.Other;

namespace SizSelCsZzz
{
    public class WebDriverExceptionMonitor
    {
        string _reporterFunction;
        IWebDriver _browser;

        public WebDriverExceptionMonitor StartMonitoring(IWebDriver browser)
        {
            var reporterFunction = "reportWebDriverExceptionMonitor" + Guid.NewGuid().ToString().Replace("-", "");

            var monitorScript = @"(function() {

    var recordedErrors = [];

    var oldOnErrorHandler = window.onerror;

    function errorHandler(errorMessage, url, line) {
        recordedErrors.push(errorMessage + ': ' + url + ' (' + line + ')');

        if (oldOnErrorHandler != null) {
            return oldOnErrorHandler.apply(this, arguments);
        }
    }
    window.onerror = errorHandler;

    window." + reporterFunction + @" = function() {
        return recordedErrors;
    }

    })();";

            DriverWrapperUnwrapper.GetDriverImplementationOf<IJavaScriptExecutor>(browser).ExecuteScript(monitorScript);

            _reporterFunction = reporterFunction;
            _browser = browser;

            return this;
        }

        public string[] GetJavascriptExceptions()
        {
            if (_browser == null)
            {
                throw new InvalidOperationException("Attempted to call WebDriverExceptionMonitor.GetJavascriptExceptions before WebDriverExceptionMonitor.StartMonitoring.");
            }

            var result = DriverWrapperUnwrapper.GetDriverImplementationOf<IJavaScriptExecutor>(_browser).ExecuteScript(
                    "return window." + _reporterFunction + "()");

            return ((System.Collections.IEnumerable)result).Cast<string>().ToArray();
        }
    }
}