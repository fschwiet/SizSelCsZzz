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
        bool _isRunningInternetExplorer = false;

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

                _isRunningInternetExplorer = true;

                var capabilities = DesiredCapabilities.InternetExplorer();
                capabilities.SetCapability("ignoreProtectedModeSettings", true);

                var browser = arrange(() => new InternetExplorerDriver(capabilities));

                cleanup(() => browser.Close());

                SpecifyForBrowser(browser);
            });
        }

        public abstract void SpecifyForBrowser(IWebDriver browser);

        public void ignoreIfInternetExplorer(string reason)
        {
            if (_isRunningInternetExplorer)
            {
                ignoreBecause(reason);
            }
        }
    }
}
