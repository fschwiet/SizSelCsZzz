using System;
using System.Collections.Generic;
using System.IO;
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
            var browserRoot = arrange(() => GetBrowsersPath());

            var allFirefoxVersions = new [] {"7.0.1", "6.0.2", "5.0.1"};
            var firstFirefoxVersion = allFirefoxVersions.First();

            given("Firefox " + firstFirefoxVersion + " with Firebug", delegate
            {
                withCategory("CommitTest");

                var exePath = GetFirefoxExe(browserRoot, firstFirefoxVersion);
                var xpiPath = GetFirebugXpi(browserRoot, firstFirefoxVersion);

                expect(() => File.Exists(exePath));

                var firefoxProfile = new FirefoxProfile();
                arrange(() => firefoxProfile.AddExtension(xpiPath));
                var browser = arrange(() => new FirefoxDriver(new FirefoxBinary(exePath), firefoxProfile));

                SpecifyForBrowser(browser);
            });

            if (Properties.Settings.Default.CommitTestsOnly)
                return;

            foreach(string version in allFirefoxVersions)
            given("Firefox " + version, delegate
            {
                var exePath = GetFirefoxExe(browserRoot, firstFirefoxVersion);

                expect(() => File.Exists(exePath));

                var browser = arrange(() => new FirefoxDriver(new FirefoxBinary(exePath), new FirefoxProfile()));

                SpecifyForBrowser(browser);
            });

            given("Chrome", delegate
            {
                var browser = arrange(() => new ChromeDriver());

                SpecifyForBrowser(browser);
            });

            given("Internet Explorer", delegate
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

        string GetFirebugXpi(string browserRoot, string firstFirefoxVersion)
        {
            return arrange(() => Directory.GetFiles(Path.Combine(browserRoot, "firefox" + firstFirefoxVersion), "*.xpi").Single());
        }

        string GetFirefoxExe(string browserRoot, string firstFirefoxVersion)
        {
            return arrange(() => Path.Combine(browserRoot, "firefox" + firstFirefoxVersion + "\\firefox.exe"));
        }

        string GetBrowsersPath()
        {
            var rootPath = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath).Directory;
            
            while (!Directory.Exists(Path.Combine(rootPath.FullName, "browser_archive")))
            {
                rootPath = rootPath.Parent;
            }

            return Path.Combine(rootPath.FullName, "browser_archive");
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
