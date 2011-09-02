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
            var server = beforeAll(() => new StaticServer("127.0.0.3", 8081)
            {
                {"Homepage.html", "<html><body>winning!</body></html>"},
                {"Homepage.htm", "<html><body>winning!</body></html>"},
                {"Inline.js", "alert('hi');"}
            }.Start());

            var webClient = arrange(() => new WebClient());

            it("can load html resources", delegate
            {
                var homepage = webClient.DownloadString("http://127.0.0.3:8081/Homepage.html");
                
                expect(() => homepage.Contains("winning!"));

                expect(() => webClient.ResponseHeaders["Content-Type"] == StaticServer.ContentType_Html);
            });

            var expectedBarContentType = "baz/example";


            describe("the server can have arbitrary resources added during test setup", delegate
            {
                beforeAll(() => server
                    .Add("foo.bar", "extra special")
                    .WithContentType("bar", expectedBarContentType));

                foreach (var kvp in new Dictionary<string, string>() {
                    {"Homepage.html", StaticServer.ContentType_Html},
                    {"Homepage.htm", StaticServer.ContentType_Html},
                    {"Homepage.HtMl", StaticServer.ContentType_Html},
                    {"foo.bar", expectedBarContentType}
                })
                {
                    it(String.Format("can load file {0} with content-type {1}", kvp.Key, kvp.Value), delegate
                    {
                        webClient.DownloadString("http://127.0.0.3:8081/" + kvp.Key);

                        expect(() => webClient.ResponseHeaders["Content-Type"] == kvp.Value);
                    });
                }
            });
        }
    }
}
