﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using SizSelCsZzz.Core;

namespace SizSelCsZzz.Test.FakeInternet
{
    public class StaticServer : FakeServer
    {
        public StaticServer(string host, int port) : base(host, port)
        {
        }

        public void Start()
        {
            base.Start((request, response) =>
            {
                var resourceName = request.Url.LocalPath.TrimStart('/');
                string body = ResourceLoader.LoadResourceRelativeToType(this.GetType(), resourceName);

                response.Headers.Add("Content-Type", "text/html; charset=UTF-8");

                using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                {
                    writer.Write(body);
                    writer.Flush();
                }
            });
        }
    }
}