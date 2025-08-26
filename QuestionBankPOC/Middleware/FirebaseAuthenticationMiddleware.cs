using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace QuestionBankPOC.Middleware
{
    public class FirebaseAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<FirebaseAuthenticationMiddleware> _logger;

        public FirebaseAuthenticationMiddleware(RequestDelegate next, ILogger<FirebaseAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    // Accessing the raw Authorization header
                    string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                    
                    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                    {
                        string token = authHeader.Substring("Bearer ".Length).Trim();
                        var jwtHandler = new JwtSecurityTokenHandler();
                        
                        if (jwtHandler.CanReadToken(token))
                        {
                            var jwtToken = jwtHandler.ReadJwtToken(token);
                            
                            // Debug output to understand what claims we're getting
                            _logger.LogInformation("JWT Token Claims:");
                            foreach (var claim in jwtToken.Claims)
                            {
                                _logger.LogInformation($"Claim: {claim.Type} = {claim.Value}");
                            }

                            // Get the role from Firebase custom claims
                            if (jwtToken.Payload.TryGetValue("role", out var role) && role != null)
                            {
                                string roleValue = role.ToString();
                                _logger.LogInformation($"Found role claim: {roleValue}");
                                
                                // Add the role claim to the current identity
                                var identity = context.User.Identity as ClaimsIdentity;
                                if (identity != null && !context.User.HasClaim(c => c.Type == "role"))
                                {
                                    identity.AddClaim(new Claim("role", roleValue));
                                    _logger.LogInformation($"Added role claim: {roleValue}");
                                }
                            }
                            else
                            {
                                _logger.LogWarning("No role claim found in JWT token");
                            }

                            // Get the companyId from Firebase custom claims
                            if (jwtToken.Payload.TryGetValue("companyId", out var companyId) && companyId != null)
                            {
                                string companyIdValue = companyId.ToString();
                                _logger.LogInformation($"Found companyId claim: {companyIdValue}");
                                
                                // Add the companyId claim to the current identity
                                var identity = context.User.Identity as ClaimsIdentity;
                                if (identity != null && !context.User.HasClaim(c => c.Type == "companyId"))
                                {
                                    identity.AddClaim(new Claim("companyId", companyIdValue));
                                    _logger.LogInformation($"Added companyId claim: {companyIdValue}");
                                }
                            }
                            else
                            {
                                _logger.LogWarning("No companyId claim found in JWT token");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Invalid JWT token format");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No Bearer token found in Authorization header");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Firebase authentication");
            }

            // Continue processing the HTTP request
            await _next(context);
        }
    }

    // Extension method for middleware registration
    public static class FirebaseAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseFirebaseAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FirebaseAuthenticationMiddleware>();
        }
    }
}
