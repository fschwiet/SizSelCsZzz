using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NJasmine;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class StaticServer_can_be_used_to_fake_external_http_dependencies : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var server = beforeAll(() => new StaticServer()
            {
                {"Homepage.html", "<html><body>winning!</body></html>"},
                {"Homepage.htm", "<html><body>winning!</body></html>"},
                {"Inline.js", "alert('hi');"}
            }.Start());

            var webClient = arrange(() => new WebClient());

            it("can load html resources", delegate
            {
                var homepage = webClient.DownloadString(server.UrlFor("Homepage.html"));
                
                expect(() => homepage.Contains("winning!"));

                expect(() => webClient.ResponseHeaders["Content-Type"] == StaticServer.ContentType_Html);
            });

            describe("the server can have arbitrary resources added during test setup", delegate
            {
                var expectedBarContentType = "baz/example";

                beforeAll(() => server
                    .Add("foo.bar", "extra special")
                    .WithContentType("bar", expectedBarContentType));

                foreach (var kvp in new Dictionary<string, string>() {
                    {"Homepage.html", StaticServer.ContentType_Html},
                    {"Homepage.htm", StaticServer.ContentType_Html},
                    {"Homepage.HtMl", StaticServer.ContentType_Html},
                    {"Inline.js", StaticServer.ContentType_Javascript},
                    {"foo.bar", expectedBarContentType}
                })
                {
                    it(String.Format("can load file {0} with content-type {1}", kvp.Key, kvp.Value), delegate
                    {
                        webClient.DownloadString(server.UrlFor(kvp.Key));

                        expect(() => webClient.ResponseHeaders["Content-Type"] == kvp.Value);
                    });
                }
            });
        }
    }
}
