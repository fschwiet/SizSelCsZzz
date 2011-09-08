using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace SizSelCsZzz.Test
{
    public abstract class SpecificationForAllBrowsers : GivenWhenThenFixture
    {
        public sealed override void Specify()
        {
            given("using Firefox", delegate
            {
                var browser = arrange(() => new FirefoxDriver());

                SpecifyForBrowser(browser);
            });

            given("using Chrome", delegate
            {
                var browser = arrange(() => new ChromeDriver());

                SpecifyForBrowser(browser);
            });

            given("using Internet Explorer", delegate
            {
                //this.ignoreBecause("InternetExplorerDriver requires particular security options.  Tools->Options->Security: 'Protected Mode' checkbox must match for all zones.");

                var capabilities = DesiredCapabilities.InternetExplorer();
                capabilities.SetCapability("ignoreProtectedModeSettings", true);

                var browser = arrange(() => new InternetExplorerDriver(capabilities));

                cleanup(() => browser.Close());

                SpecifyForBrowser(browser);
            });
        }

        public abstract void SpecifyForBrowser(IWebDriver browser);
    }
}
