using DomainModels.DB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    /// <summary>
    /// Token service for creating JWT tokens with the use of the appsettings.json file
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config) 
        {
            // Get the configuration from the appsettings.json file
            _config = config;

            // Create a symmetric security key from the secret key in the appsettings.json file
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]));
        }

        /// <summary>
        /// Create a JWT token for a user
        /// </summary>
        /// <param name="user">User object gotten through the use of a db context</param>
        /// <returns>JWT token as a string</returns>
        public string CreateToken(User user)
        {
            // Create a list of claims for the user (claims are known bits of information that can be sent in the token)
            // This is also known as the payload
            // In this case, we are sending the email and username of the user
            // But you can send any information you want really
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            // Create a signature for the token
            // This is used to verify the token when it is sent back to the server
            var signature = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Create a token descriptor
            // This is used to create the token and actually sign it
            // It contains the claims, the expiration date, the signature, and the issuer and audience
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = signature,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            // Create a token handler and create the token with it
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Write the token to a string and return it
            return tokenHandler.WriteToken(token);
        }


    }

    /// <summary>
    /// Interface for the token service
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Create a JWT token for a user
        /// </summary>
        /// <param name="user">User object gotten through the use of a db context</param>
        /// <returns>JWT token as a string</returns>
        public string CreateToken(User user);
    }

    /// <summary>
    /// Post configuration options for the JWT bearer
    /// </summary>
    /// <remarks>
    /// Currently not used, but can be used to configure the JWT bearer options
    /// </remarks>
    public sealed class JwtBearerPostConfigurationOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public JwtBearerPostConfigurationOptions(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void PostConfigure(string? name, JwtBearerOptions options)
        {
            options.Backchannel = string.IsNullOrEmpty(name) ? _httpClientFactory.CreateClient() : _httpClientFactory.CreateClient(name);
        }


    }

}
