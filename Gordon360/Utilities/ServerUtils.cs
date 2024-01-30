using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using System.Linq;

namespace Gordon360.Utilities;

public class ServerUtils
{
    private readonly IServer _server;

    public ServerUtils(IServer server)
    {
        _server = server;
    }

    public string? GetAddress()
    {
        var addresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses;
        var serverAddress = addresses?.FirstOrDefault(a => a.StartsWith("https")) ?? addresses?.FirstOrDefault();
        return serverAddress;
    }
}
