using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class miscellaneous_webdriver_extension_methods : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            describe("CountElementsMatching", delegate
            {
                var server = arrange(() => new StaticServer()
                {
                    {"list.html", "<ul><li>foo</li><li>bar</li><li>baz</li></ul"}
                }.Start());

                it("counts elements on a page", delegate
                {
                    browser.Navigate().GoToUrl(server.UrlFor("list.html"));

                    expect(() => browser.CountElementsMatching("li:contains('foo'), li:contains('bar')") == 2);
                });
            });
        }
    }
}
