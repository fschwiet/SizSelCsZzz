using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Nancy;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;
using SizSelCsZzz.jquerySource;
using SizSelCsZzz.Other;
using SizSelCsZzz.Test.jquerySource;

namespace SizSelCsZzz.Test
{
    public class JQueryAjaxWaiter_waits_for_ajax_requests : SpecificationForAllBrowsers
    {
        public class SomeTestServer : NancyModule
        {
            public static AutoResetEvent WaitHandle;

            public SomeTestServer()
            {
                Get["/homepage.html"] = c => "<html></html>";

                Get["/jquery.js"] = c => JQuerySource.GetJQuerySource();

                Get["/pageWithJQuery.html"] = c => JQueryUtil.HtmlLoadingJQuery("jquery.js");

                Get["/wait"] = c =>
                {
                    WaitHandle.WaitOne();
                    return "<html></html>";
                };
            }
        }

        public override void SpecifyForBrowser(IWebDriver browser)
        {
            var server = beforeAll(() => new NancyModuleRunner(c => c.Module<SomeTestServer>()));

            it("requires jQuery", delegate
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
                    SomeTestServer.WaitHandle = beforeEach(() => new System.Threading.AutoResetEvent(false));

                    afterEach(() => SomeTestServer.WaitHandle.Set());

                    arrange(delegate
                    {
                        browser.Navigate().GoToUrl(server.UrlFor("pageWithJQuery.html"));

                        browser.MonitorJQueryAjax();

                        var executor = browser as IJavaScriptExecutor;
                        executor.ExecuteScript("jQuery.ajax(" + Newtonsoft.Json.JsonConvert.SerializeObject(server.UrlFor("wait")) + ");");
                    });

                    ignoreIfInternetExplorer("not sure why this test isn't working on IE- may be a test only bug");

                    then("IsAjaxPending returns true", delegate
                    {
                        expect(() => browser.IsAjaxPending());
                    });

                    when("that longrunning call completes", delegate
                    {
                        arrange(() => SomeTestServer.WaitHandle.Set());

                        then("IsAjaxPending returns false", delegate
                        {
                            expect(() => !browser.IsAjaxPending());
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
