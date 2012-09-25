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
            var browserRoot = ResolvePathRelativeToDllDirectory(Properties.Settings.Default.BrowserArchivePath);

            //var isFirstVersion = true;

            foreach(string version in GetVersionByDirectoryWithPattern(browserRoot, "firefox-*"))
            given("Firefox " + version, delegate
            {
                //if (isFirstVersion)
                //{
                //    withCategory("CommitTest");
                //    isFirstVersion = false;
                //}

                var browser = beforeAll(() => LoadFirefoxDriver(browserRoot, version));

                SpecifyForBrowser(browser);
            });

            foreach(var version in GetVersionByDirectoryWithPattern(browserRoot, "chrome-*"))
            {
                given("Chrome " + version, delegate
                {
                    var browser = beforeAll(() =>
                    {
                        var exePath = Path.Combine(browserRoot, "chrome-" + version + "\\chrome-bin\\" + version + "\\chrome.exe");
                        expect(() => File.Exists(exePath));

                        return new ChromeDriver(browserRoot, new ChromeOptions()
                        {
                            BinaryLocation = exePath
                        });
                    });

                    afterAll(() => browser.Quit());

                    SpecifyForBrowser(browser);
                });
            }

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

        static IEnumerable<string> GetVersionByDirectoryWithPattern(string browserRoot, string searchPattern)
        {
            return Directory.GetDirectories(browserRoot, searchPattern)
                .Select(d => d.Substring(d.LastIndexOf("-") + 1))
                .OrderByDescending(d => int.Parse(d.Substring(0, d.IndexOf("."))));
        }

        FirefoxDriver LoadFirefoxDriver(string browserRoot, string firstFirefoxVersion)
        {
            var exePath = Path.Combine(browserRoot, "firefox-" + firstFirefoxVersion + "\\firefox.exe");

            expect(() => File.Exists(exePath));

            var profilePath = Path.Combine(Path.GetTempPath(), "EmptyProfileFolderForSizSelCsZzz_testing");
            if (Directory.Exists(profilePath))
                Directory.Delete(profilePath);

            Directory.CreateDirectory(profilePath);

            var firefoxProfile = new FirefoxProfile(profilePath);
            //arrange(() => firefoxProfile.AddExtension(xpiPath));
            return new FirefoxDriver(new FirefoxBinary(exePath), firefoxProfile);
        }

        string GetFirebugXpi(string browserRoot, string firstFirefoxVersion)
        {
            return Directory.GetFiles(Path.Combine(browserRoot, "firefox" + firstFirefoxVersion), "*.xpi").Single();
        }

        public void ignoreIfInternetExplorer(string reason)
        {
            if (_isRunningInternetExplorer)
            {
                ignoreBecause(reason);
            }
        }

        public string ResolvePathRelativeToDllDirectory(string path)
        {
            return Path.GetFullPath(Path.Combine(new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath).Directory.FullName, path));
        }
    }
}
