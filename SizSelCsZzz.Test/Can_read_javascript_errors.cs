using System.Linq;
using System.Security.Policy;
using System.Text;
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
                Get["/hello"] = c => "<html><title>OK</title></html>";
                Get["/fail"] = c =>
                    @"<html> 
                <title>FAIL</title> 
                <script> throw 'Hello, World';  </script></html>";
            }
        }

        public override void SpecifyForBrowser(IWebDriver browser)
        {
            var server = beforeAll(() => new NancyModuleRunner(new FakeServerWithJavascriptErrors()));

            var exceptionReader = arrange(() => new WebDriverExceptionMonitor().Monitor(browser));

            when("the user visits a page without errors", delegate()
            {
                arrange(() => browser.Navigate().GoToUrl(server.UrlFor("hello")));

                expect(() => browser.Title == "OK");

                then("no errors are recorded", delegate()
                {
                    expect(() => !exceptionReader.GetJavascriptExceptions().Any());
                });
            });

            //when("the user visits a page with errors", delegate()
            //{
            //    arrange(() => browser.Navigate().GoToUrl(server.UrlFor("fail")));

            //    expect(() => browser.Title == "FAIL");

            //    then("the error is recorded", delegate()
            //    {
            //        expect(() => exceptionReader.GetJavascriptExceptions().Single().Contains("Hello, world"));
            //    });
            //});
        }
    }
}
