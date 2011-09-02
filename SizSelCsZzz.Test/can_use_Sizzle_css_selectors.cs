using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace SizSelCsZzz.Test
{
    public class can_use_Sizzle_css_selectors : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            given("a server serving a simple page", delegate
            {
                var server = beforeAll(() => new StaticServer("127.0.0.3", 8081)
                    .IncludingHtml(
                        "HelloWorld.html",
                        @"
                            <html>
                            <body>
                                <div>Hello, world</div>

                                <ul>
                                    <li></li>
                                    <li></li>
                                    <li>""quotes""</li>
                                </ul>
                            </body>
                            </html>")
                    .Start());

                when("Selenium is used to test a Hello, World page", delegate
                {
                    arrange(() => browser.Navigate().GoToUrl("http://127.0.0.3:8081/HelloWorld.html"));

                    then("FindElements can be used to find an element", delegate
                    {
                        expect(() => browser.FindElements(BySizzle.CssSelector("div:contains('Hello')")).Count() == 1);
                    });

                    then("FindElement can be used to find an element", delegate
                    {
                        expect(() => browser.FindElement(BySizzle.CssSelector("div:contains('Hello')")) != null);
                    });

                    then("FindElement can handle special characters", delegate
                    {
                        expect(() => browser.FindElement(BySizzle.CssSelector("li:contains('\"quotes\"')")) != null);
                    });
                });
            });
        }
    }
}
