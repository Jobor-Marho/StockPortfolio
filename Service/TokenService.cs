using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using identity.interfaces;
using Identity.Models;
using Microsoft.IdentityModel.Tokens;

namespace identity.Service
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        // Initialize the TokenService with the configuration and the key to sign the token.
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }

        public string CreateToken(AppUser user)
        {
            // Create a list of claims to be added to the token.
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
            };

            //Create the credentials for the token inorder to sign it with the key.
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            // Create the token descriptor with the claims, expiry date, credentials, issuer and audience.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            // Create the token with the token descriptor.
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return the token as a string.
            return tokenHandler.WriteToken(token);
        }


    }
}