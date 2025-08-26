
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace QuestionBankPOC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AllRoles")]
    public class SharedController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var role = User.FindFirst("role")?.Value;
            return Ok(new { 
                message = $"Welcome to Shared Dashboard - {role}",
                role = role,
                features = new[] { "Profile Management", "Notifications", "Help" }
            });
        }
    }
}