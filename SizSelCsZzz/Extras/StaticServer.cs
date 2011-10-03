using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SizSelCsZzz.Extras
{
    public class StaticServer : FakeServer, IEnumerable
    {
        public static readonly string ContentType_Html = "text/html; charset=UTF-8";
        public static readonly string ContentType_Javascript = "text/javascript; charset=UTF-8";

        Dictionary<string, string> _contentTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        Dictionary<string, Action<RequestResponseContext>> _pathHandler = 
            new Dictionary<string, Action<RequestResponseContext>>(StringComparer.InvariantCultureIgnoreCase);

        public StaticServer()
        {
            WithContentType("html", ContentType_Html);
            WithContentType("htm", ContentType_Html);
            WithContentType("js", ContentType_Javascript);
        }

        public StaticServer Add(string path, string content)
        {
            return Add(path, context =>
            {
                using (var stream = context.Response.OutputStream)
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(content);
                    writer.Flush();
                }

            });
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

                if (_pathHandler.ContainsKey(localPath))
                {
                    AddContentTypeIfAvailable(localPath, response);

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
            });

            return this;
        }

        void AddContentTypeIfAvailable(string localPath, HttpListenerResponse response)
        {
            var extension = localPath.Substring(localPath.LastIndexOf(".") + 1);

            if (_contentTypes.ContainsKey(extension))
                response.Headers.Add("Content-Type", _contentTypes[extension]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pathHandler.GetEnumerator();
        }

        public StaticServer WithContentType(string extension, string expectedBarContentType)
        {
            _contentTypes[extension] = expectedBarContentType;

            return this;
        }
    }
}