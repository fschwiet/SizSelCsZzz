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
            ignoreIfInternetExplorer("For some reason Internet Explorer won't open a new window from this test.");

            given("a webpage that opens another window", delegate()
            {
                var server = beforeAll(() => new StaticServer()
                {
                    {"first.html", "<html>original window here<a href='/second.html' target='_blank'>hi</a></html>"},
                    {"second.html", "<html>new window here</html>"}
                }.Start());

                beforeAll(() => server.Start());

                arrange(() => browser.Navigate().GoToUrl(server.UrlFor("first.html")));

                var existingWindows = arrange(() => new WhichWindowContext(browser));

                when("the new window link is clicked", delegate()
                {
                    arrange(() => browser.FindElement(By.TagName("a")).Click());

                    arrange(delegate()
                    {
                        browser.SwitchTo().Window(existingWindows.GetNewWindowName());

                        expect(() => browser.ContainsText("new window here"));
                    });

                    then("the new window can be switch to", delegate()
                    {
                    });

                    then("the original window can be switched back to", delegate()
                    {
                        browser.SwitchTo().Window(existingWindows.GetOriginalWindowName());

                        expect(() => browser.ContainsText("original window here"));
                    });
                });
            });
        }
    }
}
