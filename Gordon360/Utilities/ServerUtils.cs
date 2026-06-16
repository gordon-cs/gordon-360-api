using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;

namespace Gordon360.Utilities;

public class ServerUtils(IServer server, IWebHostEnvironment env)
{
    public string? GetAddress()
    {
        switch (env.EnvironmentName)
        {
            case "Train":
                return "https://360ApiTrain.gordon.edu/";
            case "Production":
                return "https://360Api.gordon.edu/";
            default:
                {
                    var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
                    var serverAddress = addresses?.FirstOrDefault(a => a.StartsWith("https")) ?? addresses?.FirstOrDefault();
                    return serverAddress;
                }
        }
    }
}
