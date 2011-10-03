using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using OpenQA.Selenium.Firefox;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class StaticServer_can_have_arbitrary_path_handlers : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a server with a redirect handler", delegate
            {
                var server = beforeAll(() => new StaticServer()
                {
                    {"Redirect.html", (context) =>
                    {
                        context.Response.AddHeader("Location", context.Server.UrlFor("NewPage.html"));
                        context.Response.StatusCode = 301;
                    }},
                    {"NewPage.html", "Hello, world"}
                }.Start());

                when("a user visits the page", delegate
                {
                    var driver = arrange(() => new FirefoxDriver());
                    arrange(() => driver.Navigate().GoToUrl(server.UrlFor("Redirect.html")));

                    then("the user is redirected", delegate
                    {
                        var bodyText = driver.GetBodyText();

                        expect(() => bodyText.Contains("Hello, world"));
                    });
                });
            });
        }
    }
}
