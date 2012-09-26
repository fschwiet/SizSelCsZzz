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

            foreach (var executeable in GetVersionByDirectoryWithPattern(browserRoot, "firefox-*", "firefox.exe"))
            given("Firefox " + executeable.MajorVersion, delegate
            {
                if (executeable.NotSupported)
                {
                    withCategory("unsupported");
                }

                var browser = beforeAll(() => LoadFirefoxDriver(executeable.FullPath));

                SpecifyForBrowser(browser);
            });

            foreach(var executeable in GetVersionByDirectoryWithPattern(browserRoot, "chrome-*", "chrome.exe"))
            {
                given("Chrome " + executeable.MajorVersion, delegate
                {
                    if (executeable.NotSupported)
                    {
                        withCategory("unsupported");
                    }

                    var browser = beforeAll(() =>new ChromeDriver(browserRoot, new ChromeOptions() 
                        {
                            BinaryLocation = executeable.FullPath
                        }));

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

        public class BrowserBinary
        {
            public int MajorVersion;
            public string FullPath;
            public bool NotSupported;
        }

        static IEnumerable<BrowserBinary> GetVersionByDirectoryWithPattern(string browserRoot, string searchPattern, string executeablePath)
        {
            var directories = Directory.GetDirectories(browserRoot, searchPattern);

            var results = new List<BrowserBinary>();

            foreach(var directory in directories)
            {
                var startOfVersion = directory.Substring(directory.LastIndexOf("-") + 1);
                var version = int.Parse(startOfVersion.Substring(0, startOfVersion.IndexOf(".")));

                var browserDiscovered = new BrowserBinary()
                {
                    FullPath = Path.Combine(directory, executeablePath),
                    MajorVersion = version,
                    NotSupported = directory.ToLower().Contains("unsupported")
                };

                if (!File.Exists(browserDiscovered.FullPath))
                    throw new NotFoundException("Expected to find browser to run at " + browserDiscovered.FullPath);

                results.Add(browserDiscovered);
            }

            return results.OrderByDescending(b => b.MajorVersion);
        }

        FirefoxDriver LoadFirefoxDriver(string executeablePath)
        {
            var profilePath = Path.Combine(Path.GetTempPath(), "EmptyProfileFolderForSizSelCsZzz_testing");
            if (Directory.Exists(profilePath))
                Directory.Delete(profilePath);

            Directory.CreateDirectory(profilePath);

            var firefoxProfile = new FirefoxProfile(profilePath);
            //arrange(() => firefoxProfile.AddExtension(xpiPath));
            return new FirefoxDriver(new FirefoxBinary(executeablePath), firefoxProfile);
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
