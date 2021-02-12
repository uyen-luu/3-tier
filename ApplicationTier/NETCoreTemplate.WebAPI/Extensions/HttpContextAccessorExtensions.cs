using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace NETCoreTemplate.WebAPI.Extensions
{
    public static class HttpContextAccessorExtensions
    {
        public static string ExtractUserEmail(this IHttpContextAccessor httpContextAccessor)
        {
            var authorization = httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
            var accessToken = authorization.ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var userName = token.Claims.First(_ => _.Type == "preferred_username").Value;
            if (string.IsNullOrWhiteSpace(userName))
                throw new Exception("Unauthorized");

            return userName;
        }
    }
}
