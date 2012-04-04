using System;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using NJasmine;
using NUnit.Framework;
using Nancy;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class Can_read_javascript_errors : SpecificationForAllBrowsers
    {
        public class FakeServerWithJavascriptErrors : NancyModule
        {
            public FakeServerWithJavascriptErrors()
            {
                Get["/hello"] = c => 
                    @"<html> 
                        <title>OK</title> 
                        <body><a href='javascript:return false;' onclick='window.callNonexistingFunction()'>click for error</a></body>
                    </html>";
            }
        }

        public override void SpecifyForBrowser(IWebDriver browser)
        {
            var server = beforeAll(() => new NancyModuleRunner(new FakeServerWithJavascriptErrors()));

            var exceptionReader = arrange(() => new WebDriverExceptionMonitor());

            when("the user visits a page", delegate()
            {
                arrange(() => browser.Navigate().GoToUrl(server.UrlFor("hello")));
                arrange(() => exceptionReader.StartMonitoring(browser));

                expect(() => browser.Title == "OK");

                then("no errors are recorded", delegate()
                {
                    expect(() => !exceptionReader.GetJavascriptExceptions().Any());
                });

                when("the user clicks something that triggers an error", delegate()
                {
                    arrange(() => browser.FindElement(By.LinkText("click for error")).Click());

                    then("the error is counted", delegate()
                    {
                        expect(() => exceptionReader.GetJavascriptExceptions().Count() > 0);
                    });

                    this.ignoreIfFirefox("Firefox does not seem to record the exception detail, but rather just 'Script Error'.  Likely due to same origin policy - http://stackoverflow.com/questions/5913978/cryptic-script-error-reported-in-javascript-in-chrome-and-firefox");

                    then("the error message is recorded", delegate()
                    {
                        expect(() => exceptionReader.GetJavascriptExceptions().Any(m => m.Contains("callNonexistingFunction")));
                    });
                });
            });

            when("the user requests errors without starting monitoring", delegate
            {
                then("a helpful exception message tells them to start monitoring", delegate
                {
                    var exception = Assert.Throws<InvalidOperationException>(() => exceptionReader.GetJavascriptExceptions());

                    expect(() => exception.Message.Contains("StartMonitoring"));
                });
            });
        }
    }
}
