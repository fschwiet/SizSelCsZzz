using System;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using NJasmine;
using NUnit.Framework;
using Nancy;
using OpenQA.Selenium;
using SizSelCsZzz;
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
                        <script>
                            function causeError() {
                                setTimeout(function() {
                                    window.callNonexistingFunction()
                                }, 500);
                            }
                        </script>
                        <body><a href='javascript:void(0);' onclick='causeError()'>click for error</a></body>
                    </html>";
            }
        }

        public override void SpecifyForBrowser(IWebDriver browser)
        {
            var server = beforeAll(() => new NancyModuleRunner(c => c.Module<FakeServerWithJavascriptErrors>()));

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

                    expectEventually(() => exceptionReader.GetJavascriptExceptions().Count() > 0);

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
