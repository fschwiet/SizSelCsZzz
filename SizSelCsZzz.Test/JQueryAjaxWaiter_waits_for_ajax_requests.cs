using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;
using SizSelCsZzz.Other;

namespace SizSelCsZzz.Test
{
    public class JQueryAjaxWaiter_waits_for_ajax_requests : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            describe("IsRequestPending indicates if a request is pending", delegate
            {
                var server = arrange(() => new StaticServer("127.0.0.3",8083)
                {
                    {"homepage.html", "<html></html>"}
                });

                it("throws an error if jQuery isn't installed", delegate
                {
                    browser.Navigate().GoToUrl("http://127.0.0.3:8080/");

                    Assert.Throws<JQueryRequiredException>(delegate
                    {

                        browser.IsAjaxPending();
                    });
                });

                it("returns false initially");
            });
        }
    }
}
