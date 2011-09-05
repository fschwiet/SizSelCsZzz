using System;
using System.Net;

namespace SizSelCsZzz.Extras
{
    public class FakeServer : IDisposable
    {
        readonly int ServerWaitTimeout = 5000;

        public string BaseUrl;
        HttpListener _listener;

        public FakeServer(string host, int port)
        {
            BaseUrl = "http://" + host + ":" + port + "/";

            _listener = new HttpListener();
            _listener.Start();
            _listener.Prefixes.Add(BaseUrl);
        }

        public void Start(Action<HttpListenerRequest, HttpListenerResponse> action)
        {
            _listener.BeginGetContext(AsyncCallback(action), null); 
        }

        public string UrlFor(string path)
        {
            return BaseUrl + path.TrimStart('/');
        }

        AsyncCallback AsyncCallback(Action<HttpListenerRequest, HttpListenerResponse> action)
        {
            AsyncCallback result = null;
            
            result = delegate(IAsyncResult ar1)
            {
                if (_listener == null)
                    return;

                var incoming = _listener.EndGetContext(ar1);

                try
                {
                    action(incoming.Request, incoming.Response);

                    incoming.Response.Close();
                }
                catch (Exception)
                {
                    incoming.Response.Abort();

                    Console.WriteLine(this.GetType() + " request failed, was requesting " + incoming.Request.Url);
                }

                _listener.BeginGetContext(result, null);
            };

            return result;
        }

        public void Dispose()
        {
            if (_listener != null)
            {
                var listener = _listener;
                _listener = null;
                listener.Close();
            }
        }
    }
}
