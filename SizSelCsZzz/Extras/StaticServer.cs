using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SizSelCsZzz.Extras
{
    public class StaticServer : FakeServer, IEnumerable<KeyValuePair<string,string>>
    {
        public static readonly string ContentType_Html = "text/html; charset=UTF-8";
        public static readonly string ContentType_Javascript = "text/javascript; charset=UTF-8";

        Dictionary<string, string> _staticContent = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        Dictionary<string, string> _contentTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        Dictionary<string, Action<RequestResponseContext>> _pathHandler = new Dictionary<string, Action<RequestResponseContext>>();

        public StaticServer()
        {
            WithContentType("html", ContentType_Html);
            WithContentType("htm", ContentType_Html);
            WithContentType("js", ContentType_Javascript);
        }

        public StaticServer Add(string path, string content)
        {
            _staticContent[path.TrimStart('/')] = content;
            return this;
        }

        public class RequestResponseContext
        {
            public HttpListenerRequest Request;
            public HttpListenerResponse Response;
            public StaticServer Server;
        }

        public StaticServer Add(string path, Action<RequestResponseContext> handler)
        {
            _pathHandler[path.TrimStart('/')] = handler;
            return this;
        }


        public StaticServer Start()
        {
            base.Start((request, response) =>
            {
                var localPath = request.Url.LocalPath.TrimStart('/');

                if (localPath.ToLower() == "favicon.ico")
                {
                    response.Abort();
                    return;
                }

                string body;

                if (_staticContent.ContainsKey(localPath))
                {
                    body = _staticContent[localPath];

                    using (var stream = response.OutputStream)
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        writer.Write(body);
                        writer.Flush();
                    }
                }
                else if (_pathHandler.ContainsKey(localPath))
                {
                    _pathHandler[localPath](new RequestResponseContext()
                    {
                        Request = request,
                        Response = response,
                        Server = this
                    });
                }
                else
                {
                    response.StatusCode = (int) HttpStatusCode.NotFound;
                    response.StatusDescription = "StaticServer did not have resource for '" + localPath + "'.";

                    return;
                }

                var extension = localPath.Substring(localPath.LastIndexOf(".") + 1);

                if (_contentTypes.ContainsKey(extension))
                    response.Headers.Add("Content-Type", _contentTypes[extension]);
                
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