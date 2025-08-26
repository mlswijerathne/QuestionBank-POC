using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace QuestionBankPOC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "EvaluatorOrAdmin")]
    public class EvaluatorController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Ok(new { 
                message = "Welcome to Evaluator Dashboard",
                role = User.FindFirst("role")?.Value,
                features = new[] { "Create Questions", "Manage Evaluations", "View Reports" }
            });
        }
    }
}
