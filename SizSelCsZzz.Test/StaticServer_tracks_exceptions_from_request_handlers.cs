using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class StaticServer_tracks_exceptions_from_request_handlers : GivenWhenThenFixture
    {
        public override void Specify()
        {
            int failureIndex = 1;

            var sut = arrange(() => new StaticServer()
            {
                {"/", context => { throw new Exception("FAIL: " + failureIndex++); }}
            }.Start());

            it("records exceptions thrown by an event handler", delegate
            {
                var driver = arrange(() => new FirefoxDriver());
                driver.Navigate().GoToUrl(sut.UrlFor("/"));
                driver.Navigate().GoToUrl(sut.UrlFor("/"));

                Assert.That(sut.Exceptions.Select(e => e.Message).ToArray(), Is.EquivalentTo(new string[] {"FAIL: 1", "FAIL: 2"}));
            });            
        }

    }
}
