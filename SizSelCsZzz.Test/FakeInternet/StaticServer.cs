using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using SizSelCsZzz.Core;

namespace SizSelCsZzz.Test.FakeInternet
{
    public class StaticServer : FakeServer
    {
        // I don't know why but loading the resource multiple times would fail...
        // So I cache them.
        //Dictionary<string, string> _resourceCache = new Dictionary<string, string>();

        public StaticServer(string host, int port) : base(host, port)
        {
        }

        public void Start()
        {
            base.Start((request, response) =>
            {
                var resourceName = request.Url.LocalPath.TrimStart('/');
                string body = null;

                //if (_resourceCache.ContainsKey(resourceName))
                //{
                //    body = _resourceCache[resourceName];
                //}
                //else
                //{
                    body = ResourceLoader.LoadResourceRelativeToType(this.GetType(), resourceName);
                //    _resourceCache[resourceName] = body;
                //}

                using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                {
                    writer.Write(body);
                    writer.Flush();
                }
            });
        }
    }
}