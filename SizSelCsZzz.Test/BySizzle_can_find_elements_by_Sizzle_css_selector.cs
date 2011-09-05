using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class BySizzle_can_find_elements_by_Sizzle_css_selector : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            given("a server serving a simple page", delegate
            {
                var server = beforeAll(() => new StaticServer("127.0.0.3", 8081)
                {
                    {"HelloWorld.html", @"
                        <html>
                        <body>
                            <div>Hello, world</div>

                            <ul>
                                <li></li>
                                <li></li>
                                <li>""quotes""</li>
                            </ul>
                        </body>
                        </html>"}
                }.Start());

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

                    then("FindElement reports a useful error if the element is not found", delegate
                    {
                        var e = Assert.Throws<NoSuchElementException>(delegate
                        {
                            browser.FindElement(BySizzle.CssSelector("div:contains('This solves everything')"));
                        });

                        expect(() => e.Message.Contains("Could not find element matching css selector '\"div:contains('This solves everything')\"'"));
                    });
                });
            });
        }
    }
}
