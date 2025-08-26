using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace QuestionBankPOC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            // Log user claims for debugging
            _logger.LogInformation("GetDashboard endpoint accessed");
            
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("User is authenticated");
                
                // Log all claims
                var claims = User.Claims.ToList();
                foreach (var claim in claims)
                {
                    _logger.LogInformation($"User claim: {claim.Type} = {claim.Value}");
                }

                // Check for role claim
                var roleClaim = User.FindFirst("role");
                _logger.LogInformation(roleClaim != null
                    ? $"Role claim found: {roleClaim.Value}"
                    : "No role claim found");

                // Manual check for admin role
                bool isAdmin = User.HasClaim(c => c.Type == "role" && c.Value == "admin");
                _logger.LogInformation($"IsAdmin from claims check: {isAdmin}");

                // Check for authentication schemes
                var authScheme = HttpContext.User.Identity?.AuthenticationType;
                _logger.LogInformation($"Authentication scheme: {authScheme}");

                return Ok(new { 
                    message = "Welcome to Admin Dashboard",
                    role = roleClaim?.Value ?? "unknown",
                    features = new[] { "User Management", "Company Settings", "Analytics" },
                    claims = claims.Select(c => new { c.Type, c.Value }),
                    isAuthenticated = User.Identity.IsAuthenticated,
                    authenticationType = User.Identity.AuthenticationType
                });
            }
            else
            {
                _logger.LogWarning("User is not authenticated");
                return Unauthorized("User is not authenticated");
            }
        }
    }
}