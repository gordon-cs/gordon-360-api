using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Thinktecture.IdentityModel.Tokens;
using System.IdentityModel.Tokens;

namespace Gordon360.AuthorizationServer
{
    public class CustomJWTFormat : ISecureDataFormat<AuthenticationTicket>
   {
        private static readonly byte[] _secret = TextEncodings.Base64Url.Decode(System.Web.Configuration.WebConfigurationManager.AppSettings["jwtSecret"]);
        private readonly string _issuer;

        public CustomJWTFormat(string issuer)
        {
            _issuer = issuer;
        }


        public string Protect(AuthenticationTicket data)
        {
            if(data == null)
            {
                throw new ArgumentNullException();
            }
            var signingKey = new HmacSigningCredentials(_secret);
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_issuer, _issuer, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey));

        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}