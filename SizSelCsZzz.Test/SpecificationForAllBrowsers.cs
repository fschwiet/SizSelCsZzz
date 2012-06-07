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
        bool _isRunningFirefox = true;

        public abstract void SpecifyForBrowser(IWebDriver browser);

        public sealed override void Specify()
        {
            var browserRoot = arrange(() => Properties.Settings.Default.BrowserArchivePath);

            var allFirefoxVersions = new [] {"13.0", "11.0", "8.0", "7.0.1", "6.0.2", "5.0.1"};
            var firstFirefoxVersion = allFirefoxVersions.First();

            var allChromeVersions = new[] { "18.0.1025.142", "14.0.835.202", "13.0.782.220", "12.0.742.112" };

            given("Firefox " + firstFirefoxVersion + " with Firebug", delegate
            {
                withCategory("CommitTest");

                _isRunningFirefox = true;

                var exePath = GetFirefoxExe(browserRoot, firstFirefoxVersion);
//                var xpiPath = GetFirebugXpi(browserRoot, firstFirefoxVersion);

                expect(() => File.Exists(exePath));

                var firefoxProfile = new FirefoxProfile();
//                arrange(() => firefoxProfile.AddExtension(xpiPath));
                var browser = arrange(() => new FirefoxDriver(new FirefoxBinary(exePath), firefoxProfile));

                SpecifyForBrowser(browser);
            });

            if (Properties.Settings.Default.CommitTestsOnly)
                return;

            foreach(string version in allFirefoxVersions.Skip(1))
            given("Firefox " + version, delegate
            {
                var exePath = GetFirefoxExe(browserRoot, firstFirefoxVersion);

                expect(() => File.Exists(exePath));

                var browser = arrange(() => new FirefoxDriver(new FirefoxBinary(exePath), new FirefoxProfile()));

                SpecifyForBrowser(browser);
            });

            foreach(var version in allChromeVersions)
            given("Chrome " + version, delegate
            {
                var browser = arrange(() =>
                {
                    var exePath = Path.Combine(Properties.Settings.Default.BrowserArchivePath, "chrome_" + version + "\\chrome-bin\\" + version + "\\chrome.exe");
                    expect(() => File.Exists(exePath));
                    
                    return new ChromeDriver(GetTestBinDeploymentDirectory(), new ChromeOptions()
                    {
                        BinaryLocation = exePath
                    });
                });

                cleanup(() => browser.Quit());

                SpecifyForBrowser(browser);
            });

            given("Internet Explorer", delegate
            {
                _isRunningInternetExplorer = true;

                var browser = arrange(() => new InternetExplorerDriver(new InternetExplorerOptions()
                {
                    IntroduceInstabilityByIgnoringProtectedModeSettings = true
                }));

                cleanup(() => browser.Quit());

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

        public static string GetTestBinDeploymentDirectory()
        {
            return new FileInfo(new Uri(typeof(SpecificationForAllBrowsers).Assembly.CodeBase).LocalPath).Directory.FullName;
        }

        public void ignoreIfInternetExplorer(string reason)
        {
            if (_isRunningInternetExplorer)
            {
                ignoreBecause(reason);
            }
        }

        public void ignoreIfFirefox(string reason)
        {
            if (_isRunningFirefox)
            {
                ignoreBecause(reason);
            }
        }
    }
}
