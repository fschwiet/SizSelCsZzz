using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using NJasmine;
using NUnit.Framework;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class Can_read_javascript_errors : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            var server = beforeAll(() => new StaticServer());
            beforeAll(() => server.Add("/hello/", "<html><title>OK</title></html>"));
            
            beforeAll(() => server.Add("/fail/", @"<html> 
                <title>FAIL</title> 
                <script> throw 'Hello, World';  </script></html>"));

            beforeAll(() => server.Start());

            var exceptionReader = arrange(() => new WebDriverExceptionMonitor().Monitor(browser));

            when("the user visits a page without errors", delegate()
            {
                arrange(() => browser.Navigate().GoToUrl(server.UrlFor("/hello/")));

                expect(() => browser.Title == "OK");

                then("no errors are recorded", delegate()
                {
                    expect(() => !exceptionReader.GetJavascriptExceptions().Any());
                });
            });

            when("the user visits a page with errors", delegate()
            {
                arrange(() => browser.Navigate().GoToUrl(server.UrlFor("/fail/")));

                expect(() => browser.Title == "FAIL");

                then("the error is recorded", delegate()
                {
                    expect(() => exceptionReader.GetJavascriptExceptions().Single().Contains("Hello, world"));
                });
            });
        }
    }

    public class WebDriverExceptionMonitor
    {
        public WebDriverExceptionMonitor()
        {
            throw new NotImplementedException();
        }

        public WebDriverExceptionMonitor Monitor(IWebDriver browser)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetJavascriptExceptions()
        {
            throw new NotImplementedException();
        }
    }
}
