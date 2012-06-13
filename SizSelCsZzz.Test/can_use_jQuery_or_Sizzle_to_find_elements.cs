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
    public class can_use_jQuery_or_Sizzle_to_find_elements : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            var server = beforeAll(() => new StaticServer()
            {
                {
                    "HelloWorld.html",
                    @"
                    <html>
                    <body>
                        <div>Hello, world</div>

                        <ul>
                            <li></li>
                            <li></li>
                            <li>""quotes""</li>
                            <li class='last_li'></li>
                        </ul>

                        <input value='Hello world'/>
                        <input value='Hello, world.'/>
                        <input id='fee-fi_foe-fum'></input>
                        <span>phrase with spaces, and commas.</span>
                    </body>
                    </html>"
                    }
            }.Start());

            beforeAll(
                () => server.Add("somepage.html", "<html><body><ul><li>LIST ITEM</li></ul><p>PARAGRAPH</p></body></html>"));

            Check(browser, server, "jQuery", ByJQuery.CssSelector);
            Check(browser, server, "Sizzle", BySizzle.CssSelector);
        }

        void Check(IWebDriver browser, StaticServer server, string selectorName, Func<string, By> cssSelector)
        {
            given("using css selector " + selectorName, () =>
            {
                describe("BySizzle.CssSelector can find elements by Sizzle CSS selector", delegate
                {
                    arrange(() => browser.Navigate().GoToUrl(server.UrlFor("HelloWorld.html")));

                    it("can be used with FindElements", delegate
                    {
                        expect(() => browser.FindElements(cssSelector("div:contains('Hello')")).Count() == 1);
                        expect(() => browser.FindElements(cssSelector("li.last_li")).Count() == 1);
                        expect(() => browser.FindElements(cssSelector("div:contains('Hello'), li.last_li")).Count() == 2);
                    });

                    it("can be used with FindElement",
                       delegate { expect(() => browser.FindElement(cssSelector("div:contains('Hello')")) != null); });

                    it("can handle special characters", delegate
                    {
                        expect(() => browser.FindElement(cssSelector("li:contains('\"quotes\"')")) != null);
                        expect(() => browser.FindElements(cssSelector("span:contains('phrase with spaces')")).Count() == 1);
                        expect(
                            () => browser.FindElements(cssSelector("span:contains('phrase with spaces, and commas.')")).Count() == 1);
                        expect(() => browser.FindElements(cssSelector("input#fee-fi_foe-fum")).Count() == 1);
                        expect(() => browser.FindElements(cssSelector("input[value='Hello world']")).Count() == 1);
                        //expect(() => browser.FindElements(By.CssSelector("input[value='Hello, world.']")).Count() == 1);
                        expect(() => browser.FindElements(cssSelector("input[value='Hello, world.']")).Count() == 1);
                    });

                    it("reports a useful error if the element is not found", delegate
                    {
                        var e =
                            Assert.Throws<NoSuchElementException>(
                                delegate { browser.FindElement(cssSelector("div:contains('This solves everything')")); });

                        expect(
                            () =>
                            e.Message.Contains(
                                "Could not find element matching css selector '\"div:contains('This solves everything')\"'"));
                    });
                });

                describe("BySizzle.CssSelector can be used transitively", delegate
                {
                    arrange(() => browser.Navigate().GoToUrl(server.UrlFor("somepage.html")));

                    it("matches descendant nodes",
                       delegate
                       {
                           expect(
                               () =>
                               browser.FindElement(cssSelector("ul")).FindElement(cssSelector("li:contains('LIST ITEM')")) != null);
                       });

                    it("does not match nondescendant nodes", delegate
                    {
                        By paragraphFinder = cssSelector("p:contains('PARAGRAPH')");

                        expect(() => browser.FindElement(paragraphFinder) != null);

                        Assert.Throws<NoSuchElementException>(
                            delegate { expect(() => browser.FindElement(cssSelector("ul")).FindElement(paragraphFinder) == null); });
                    });
                });
            });
        }
    }
}
