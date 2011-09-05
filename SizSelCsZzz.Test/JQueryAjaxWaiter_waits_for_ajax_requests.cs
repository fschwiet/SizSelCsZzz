using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;
using SizSelCsZzz.Other;
using SizSelCsZzz.Test.jquerySource;

namespace SizSelCsZzz.Test
{
    public class JQueryAjaxWaiter_waits_for_ajax_requests : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            var server = arrange(() => new StaticServer("127.0.0.3", 8083)
                {
                    {"homepage.html", "<html></html>"},
                    {"pageWithJQuery.html", JQueryUtil.HtmlLoadingJQuery("http://127.0.0.3:8083/jquery.js")},
                    {"jquery.js",JQueryUtil.GetJQuerySource()},
                }.Start());

            it("requires javascript", delegate
            {
                browser.Navigate().GoToUrl(server.UrlFor("homepage.html"));

                Assert.Throws<JQueryNotInstalledException>(delegate
                {
                    browser.MonitorJQueryAjax();
                });

                Assert.Throws<JQueryNotInstalledException>(delegate
                {
                    browser.IsAjaxPending();
                });
            });

            describe("MonitorJQueryAjax", delegate
            {
                it("must be called before waiting on ajax", delegate
                {
                    browser.Navigate().GoToUrl(server.UrlFor("pageWithJQuery.html"));

                    var exceptions = Assert.Throws<InvalidOperationException>(delegate
                    {
                        expect(() => !browser.IsAjaxPending());
                    });

                    expect(() => exceptions.Message.Contains("Must call MonitorJQueryAjax before waiting on ajax."));
                });

                it("installs some javascript code for monitoring", delegate
                {
                    browser.Navigate().GoToUrl(server.UrlFor("pageWithJQuery.html"));

                    expect(() => !JQueryAjaxWaiter.WasMonitoringStarted(browser));

                    browser.MonitorJQueryAjax();

                    expect(() => JQueryAjaxWaiter.WasMonitoringStarted(browser));
                });

                it("doesnt reinstall that javascript code", delegate
                {
                    browser.Navigate().GoToUrl(server.UrlFor("pageWithJQuery.html"));
                    
                    browser.MonitorJQueryAjax();

                    var scriptExecutor = browser as IJavaScriptExecutor;
                    scriptExecutor.ExecuteScript("window.SizSelCsZzz_IsRequestPending.tracer = true;");

                    browser.MonitorJQueryAjax();

                    expect(() => (bool)scriptExecutor.ExecuteScript("return window.SizSelCsZzz_IsRequestPending.tracer;"));
                });
            });

            describe("IsRequestPending indicates if an AJAX request is pending", delegate
            {
                it("returns false initially", delegate
                {
                    browser.Navigate().GoToUrl(server.UrlFor("pageWithJQuery.html"));

                    browser.MonitorJQueryAjax();

                    expect(() => !browser.IsAjaxPending());
                });

                when("the browser starts a long-running ajax operation", delegate
                {
                    var waitHandle = beforeAll(() => new System.Threading.AutoResetEvent(false));

                    var slowServer = beforeAll(delegate
                    {
                        var result = new FakeServer("127.0.0.4", 8083);
                        result.Start((request, response) =>
                        {
                            waitHandle.WaitOne();
                            response.Close();
                        });
                        return result;
                    });
                    
                    arrange(delegate
                    {
                        browser.Navigate().GoToUrl(server.UrlFor("pageWithJQuery.html"));

                        browser.MonitorJQueryAjax();

                        var executor = browser as IJavaScriptExecutor;
                        executor.ExecuteScript("jQuery.ajax(" + Newtonsoft.Json.JsonConvert.SerializeObject(slowServer.UrlFor("")) + ");");
                    });

                    then("IsAjaxPending returns true", delegate
                    {
                        expect(() => browser.IsAjaxPending());
                    });

                    when("that longrunning call completes", delegate
                    {
                        arrange(() => waitHandle.Set());

                        then("IsAjaxPending returns false", delegate
                        {
                            expect(() => browser.IsAjaxPending());
                        });
                    });
                });
            });

            describe("StartJQueryAjaxCheck", delegate
            {
                ignoreBecause("not implemented");
                it("requires monitoring was started");

                describe("StartJQueryAjaxCheck.HasCompleted.", delegate
                {
                    it("returns false if no ajax requests are pending");
                    it("returns true in an ajax request was pending while it was created");
                    it("returns false if ajax request starts but does not finish");
                    it("returns true if an ajax request starts and finishes");
                });
            });
        }
    }
}
