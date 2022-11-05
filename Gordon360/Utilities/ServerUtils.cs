using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using System.Linq;

namespace Gordon360.Utilities
{
    public class ServerUtils
    {
        private readonly IServer _server;

        public ServerUtils(IServer server)
        {
            _server = server;
        }

        public string GetAddress()
        {
            var baseUrl = _server.Features.Get<IServerAddressesFeature>()?.Addresses.FirstOrDefault(a => a.Contains("https"));
            return baseUrl;
        }
    }
}
