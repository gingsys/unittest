using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;

namespace SisConAxs.Session
{
    public class AuthenticationProviderAzureAD : IAuthenticationProvider
    {
        public string GetLoginUsername(string username)
        {
            return username;
        }

        // por mejorar, lo ideal sería validarlo con una consulta al servidor de Azure pero se generan conflictos en la ejecución, se necesita a futuro migrar el Proyecto Web a Net Core con el q se tiene mejor compatibilidad
        public bool Validate(string username, string credentials)
        {
            var token = new JwtSecurityToken(jwtEncodedString: credentials);
            string claimTenantId = token.Claims.First(p => p.Type == "tid").Value.ToLower().Trim();
            string claimPreferredUsername = token.Claims.First(p => p.Type == "preferred_username").Value.ToLower().Trim();
            string tenantId = ConfigurationManager.AppSettings["AzureTenantId"].ToLower().Trim();
            username = username.ToLower().Trim();

            return tenantId == claimTenantId && claimPreferredUsername == username;
            //var secToken = Validate(credentials);
            //return secToken.ValidFrom >= DateTime.Now && DateTime.Now >= secToken.ValidTo;
        }


        //public JwtSecurityToken Validate(string token)
        //{
        //    IdentityModelEventSource.ShowPII = true;

        //    //string stsDiscoveryEndpoint = $"https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
        //    string stsDiscoveryEndpoint = $"https://login.microsoftonline.com/{}/v2.0/.well-known/openid-configuration";
        //    ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
        //    OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().Result;

        //    TokenValidationParameters validationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = false,
        //        ValidateIssuer = false,
        //        //IssuerSigningTokens = config.SigningTokens,
        //        IssuerSigningKeys = config.SigningKeys,
        //        ValidateLifetime = false
        //    };

        //    JwtSecurityTokenHandler tokendHandler = new JwtSecurityTokenHandler();
        //    SecurityToken jwt;
        //    var result = tokendHandler.ValidateToken(token, validationParameters, out jwt);
        //    return jwt as JwtSecurityToken;
        //}
    }
}