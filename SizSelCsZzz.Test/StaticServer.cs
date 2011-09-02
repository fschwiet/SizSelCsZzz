using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SizSelCsZzz.Core;

namespace SizSelCsZzz.Test
{
    public class StaticServer : FakeServer
    {
        Dictionary<string,string> _staticContent = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public StaticServer(string host, int port) : base(host, port)
        {
        }

        public StaticServer IncludingHtml(string path, string content)
        {
            _staticContent[path] = content;
            return this;
        }

        public StaticServer Start()
        {
            base.Start((request, response) =>
            {
                var resourceName = request.Url.LocalPath.TrimStart('/');

                if (resourceName.ToLower() == "favicon.ico")
                {
                    response.Abort();
                    return;
                }

                string body;


                if (_staticContent.ContainsKey(resourceName))
                {
                    body = _staticContent[resourceName];
                }
                else
                {
                    body = ResourceLoader.LoadResourceRelativeToType(this.GetType(), resourceName);
                }

                response.Headers.Add("Content-Type", "text/html; charset=UTF-8");

                using (var stream = response.OutputStream)
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(body);
                    writer.Flush();
                }
            });

            return this;
        }
    }
}