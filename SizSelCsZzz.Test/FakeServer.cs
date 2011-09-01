using System;
using System.Net;

namespace SizSelCsZzz.Test
{
    public class FakeServer : IDisposable
    {
        readonly int ServerWaitTimeout = 5000;

        HttpListener _listener;

        public FakeServer(string host, int port)
        {
            _listener = new HttpListener();
            _listener.Start();
            _listener.Prefixes.Add("http://" + host + ":" + port + "/");
        }

        public void Start(Action<HttpListenerRequest, HttpListenerResponse> action)
        {
            _listener.BeginGetContext(AsyncCallback(action), null); 
        }

        AsyncCallback AsyncCallback(Action<HttpListenerRequest, HttpListenerResponse> action)
        {
            AsyncCallback result = null;
            
            result = delegate(IAsyncResult ar1)
            {
                if (_listener == null)
                    return;

                var incoming = _listener.EndGetContext(ar1);

                action(incoming.Request, incoming.Response);

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
