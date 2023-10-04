using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MedTracker.Midleware
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                // Validate and decode the token here
                var claims = ValidateAndDecodeToken(token);

                if (claims != null)
                {
                    // Add claims to the current identity if the token is valid
                    var identity = new ClaimsIdentity(claims, "bearer");
                    context.User = new ClaimsPrincipal(identity);
                }
            }

            // Continue processing the request
            await _next(context);
        }

        private List<Claim>? ValidateAndDecodeToken(string tokenString)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadToken(tokenString) as JwtSecurityToken;

            if (token != null)
            {
                var claims = new List<Claim>();
                foreach (var claim in token.Claims)
                {
                    claims.Add(claim);
                }

                return claims;
            }

            return null;
        }
    }
}
