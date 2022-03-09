using System.Threading.Tasks;
using System.DirectoryServices;
using Microsoft.Extensions.Options;
using Gordon360.Configuration;

namespace Gordon360.Services
{
    public class LdapAuthenticationService : IAuthenticationService
    {
        private const string DisplayNameAttribute = "DisplayName";
        private const string SAMAccountNameAttribute = "SAMAccountName";

        private readonly LdapConfig _config;

        public LdapAuthenticationService(IOptions<LdapConfig> config)
        {
            _config = config.Value;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            using DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(_config.Path, $"{_config.UserDomainName}\\{username}", password);
        }
    }
}
