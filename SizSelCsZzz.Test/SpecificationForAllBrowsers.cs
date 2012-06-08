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

        public abstract void SpecifyForBrowser(IWebDriver browser);

        public sealed override void Specify()
        {
            var browserRoot = beforeAll(() => Properties.Settings.Default.BrowserArchivePath);

            var allFirefoxVersions = new [] {"13.0", "11.0", "8.0", "7.0.1", "6.0.2", "5.0.1"};
            var firstFirefoxVersion = allFirefoxVersions.First();

            var allChromeVersions = new[] { "18.0.1025.142", "14.0.835.202", "13.0.782.220", "12.0.742.112" };

            given("Firefox " + firstFirefoxVersion + " with Firebug", delegate
            {
                withCategory("CommitTest");

                var browser = beforeAll(() => LoadFirefoxDriver(browserRoot, firstFirefoxVersion));

                SpecifyForBrowser(browser);
            });

            if (Properties.Settings.Default.CommitTestsOnly)
                return;

            foreach(string version in allFirefoxVersions.Skip(1))
            given("Firefox " + version, delegate
            {
                var browser = beforeAll(() => LoadFirefoxDriver(browserRoot, firstFirefoxVersion));

                SpecifyForBrowser(browser);
            });

            foreach(var version in allChromeVersions)
            given("Chrome " + version, delegate
            {
                var browser = beforeAll(() =>
                {
                    var exePath = Path.Combine(Properties.Settings.Default.BrowserArchivePath, "chrome_" + version + "\\chrome-bin\\" + version + "\\chrome.exe");
                    expect(() => File.Exists(exePath));
                    
                    return new ChromeDriver(GetTestBinDeploymentDirectory(), new ChromeOptions()
                    {
                        BinaryLocation = exePath
                    });
                });

                afterAll(() => browser.Quit());

                SpecifyForBrowser(browser);
            });

            given("Internet Explorer", delegate
            {
                _isRunningInternetExplorer = true;

                var browser = beforeAll(() => new InternetExplorerDriver(new InternetExplorerOptions()
                {
                    IntroduceInstabilityByIgnoringProtectedModeSettings = true
                }));

                afterAll(() => browser.Quit());

                SpecifyForBrowser(browser);
            });
        }

        FirefoxDriver LoadFirefoxDriver(string browserRoot, string firstFirefoxVersion)
        {
            var exePath = Path.Combine(browserRoot, "firefox" + firstFirefoxVersion + "\\firefox.exe");

            expect(() => File.Exists(exePath));

            var firefoxProfile = new FirefoxProfile();
            //arrange(() => firefoxProfile.AddExtension(xpiPath));
            return new FirefoxDriver(new FirefoxBinary(exePath), firefoxProfile);
        }

        string GetFirebugXpi(string browserRoot, string firstFirefoxVersion)
        {
            return Directory.GetFiles(Path.Combine(browserRoot, "firefox" + firstFirefoxVersion), "*.xpi").Single();
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
    }
}
