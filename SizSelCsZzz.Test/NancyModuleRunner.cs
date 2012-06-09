using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Hosting.Self;
using Nancy.Testing;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test
{
    public class NancyModuleRunner : IDisposable
    {
        public string BaseUrl;
        NancyHost _nancyHost;

        public NancyModuleRunner(Action<ConfigurableBootstrapper.ConfigurableBoostrapperConfigurator> configuration)
        {
            var host = "localhost";
            int port = TCPUtil.GetAvailableTCPPort(8081, 8090);

            BaseUrl = "http://" + host + ":" + port + "/";

            var bootStrapper = new ConfigurableBootstrapper(configuration);

            _nancyHost = new NancyHost(bootStrapper, new Uri(BaseUrl));
            _nancyHost.Start();
        }

        public string UrlFor(string path)
        {
            return BaseUrl + path.TrimStart('/');
        }

        public void Dispose()
        {
            if (_nancyHost != null)
            {
                _nancyHost.Stop();
                _nancyHost = null;
            }
        }
    }
}
