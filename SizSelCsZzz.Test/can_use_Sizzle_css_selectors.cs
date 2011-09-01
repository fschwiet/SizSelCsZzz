using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SizSelCsZzz.Test.FakeInternet;

namespace SizSelCsZzz.Test
{
    public class can_use_Sizzle_css_selectors : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a server serving a simple page", delegate
            {
                var server = arrange(() => new StaticServer("127.0.0.3", 8081));
                arrange(() => server.Start());

                when("we try to find an element using a Sizzler specific selector", delegate
                {
                    var browser = arrange(() => new FirefoxDriver());
                    arrange(() => browser.Navigate().GoToUrl("http://127.0.0.3:8081/HelloWorld.html"));

                    then("we can find the element with FindElements", delegate
                    {
                        expect(() => browser.FindElements(BySizzler.CssSelector("div:contains('Hello')")).Count() == 1);
                    });

                    then("we can find the element with FindElement", delegate
                    {
                        expect(() => browser.FindElement(BySizzler.CssSelector("div:contains('Hello')")) != null);
                    });
                });
            });

            // should handle special characters
            // should use .First() vs .Single() like By.CssSelector
            // should fail like By.CssSelector
        }
    }
}
