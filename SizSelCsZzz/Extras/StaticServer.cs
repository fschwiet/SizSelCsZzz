using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SizSelCsZzz.Extras
{
    public class StaticServer : FakeServer, IEnumerable<KeyValuePair<string,string>>
    {
        public static readonly string ContentType_Html = "text/html; charset=UTF-8";

        Dictionary<string, string> _staticContent = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        Dictionary<string, string> _contentTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public StaticServer(string host, int port) : base(host, port)
        {
            WithContentType("html", ContentType_Html);
            WithContentType("htm", ContentType_Html);
        }

        public StaticServer Add(string path, string content)
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

                var extension = resourceName.Substring(resourceName.LastIndexOf(".") + 1);

                if (_contentTypes.ContainsKey(extension))
                    response.Headers.Add("Content-Type", _contentTypes[extension]);

                using (var stream = response.OutputStream)
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(body);
                    writer.Flush();
                }
            });

            return this;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _staticContent.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _staticContent.GetEnumerator();
        }

        public StaticServer WithContentType(string extension, string expectedBarContentType)
        {
            _contentTypes[extension] = expectedBarContentType;

            return this;
        }
    }
}