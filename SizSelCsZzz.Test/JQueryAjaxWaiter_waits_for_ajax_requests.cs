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
                var jquery = beforeAll(() => ResourceLoader.LoadResourceRelativeToType(this.GetType(), "jquerySource.jquery-1.6.2.js"));
                expect(() => !string.IsNullOrEmpty(jquery));

                var server = arrange(() => new StaticServer("127.0.0.3",8083)
                {
                    {"homepage.html", "<html></html>"},
                    {"pageWithJQuery.html", @"<html>
<script src='http://127.0.0.3:8083/jquery.js'></script>
</html>"},
                    {"jquery.js",jquery},
                }.Start());

                it("throws an error if jQuery isn't installed", delegate
                {
                    browser.Navigate().GoToUrl("http://127.0.0.3:8083/homepage.html");

                    Assert.Throws<JQueryNotInstalledException>(delegate
                    {
                        browser.IsAjaxPending();
                    });
                });

                it("returns false initially", delegate
                {
                    browser.Navigate().GoToUrl("http://127.0.0.3:8083/pageWithJQuery.html");

                    expect(() => false == browser.IsAjaxPending());
                });
            });
        }
    }
}
