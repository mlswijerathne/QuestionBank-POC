using Microsoft.AspNetCore.Mvc;

namespace QuestionBankPOC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly ILogger<DebugController> _logger;

        public DebugController(ILogger<DebugController> logger)
        {
            _logger = logger;
        }

        [HttpGet("token")]
        public IActionResult GetTokenInfo()
        {
            // Log all headers for debugging
            _logger.LogInformation("Debug token endpoint accessed");
            foreach (var header in Request.Headers)
            {
                _logger.LogInformation($"Request header: {header.Key} = {header.Value}");
            }

            // Check if Authorization header exists
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogWarning("No Authorization header found");
                return Ok(new { 
                    authenticated = false,
                    message = "No Authorization header found",
                    headers = Request.Headers.Select(h => new { h.Key, Value = h.Value.ToString() })
                });
            }

            // Log user claims if authenticated
            var isAuthenticated = User.Identity?.IsAuthenticated == true;
            _logger.LogInformation($"User is authenticated: {isAuthenticated}");
            
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            
            return Ok(new { 
                authenticated = isAuthenticated,
                authHeader = Request.Headers["Authorization"].ToString(),
                claims = claims,
                authType = User.Identity?.AuthenticationType,
                message = "Token information"
            });
        }
    }
}
