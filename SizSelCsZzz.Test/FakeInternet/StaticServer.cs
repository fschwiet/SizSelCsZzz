using System.IO;
using System.Net;
using System.Text;

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
                using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                {
                    var resourceContent = ResourceLoader.LoadResourceRelativeToType(this.GetType(),
                        request.Url.LocalPath.TrimStart('/'));
                    writer.Write(resourceContent);
                }
            });
        }
    }
}