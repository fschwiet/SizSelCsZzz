using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class Can_switch_to_new_window : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            given("a webpage that opens another window", delegate()
            {
                var server = beforeAll(() => new StaticServer()
                {
                    {"first.html", "<html><a href='/second.html' target='_blank'>hi</a></html>"},
                    {"second.html", "<html>new window here</html>"}
                }.Start());

                beforeAll(() => server.Start());

                arrange(() => browser.Navigate().GoToUrl(server.UrlFor("first.html")));

                var existingWindows = arrange(() => new ExistingWindow(browser));

                when("the new window link is clicked", delegate()
                {
                    arrange(() => browser.FindElement(By.TagName("a")).Click());

                    then("a new window is opened", delegate()
                    {
                        browser.SwitchTo().Window(existingWindows.GetNewWindowName());

                        expect(() => browser.ContainsText("new window here"));
                    });
                });
            });
        }
    }
}
