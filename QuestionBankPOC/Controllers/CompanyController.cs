using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuestionBankPOC.Services;
using QuestionBankPOC.DTOs;

namespace QuestionBankPOC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IFirebaseService _firebaseService;
        
        public CompanyController(ICompanyService companyService, IFirebaseService firebaseService)
        {
            _companyService = companyService;
            _firebaseService = firebaseService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyRequest request)
        {
            try
            {
                // Verify Firebase token
                var firebaseUid = await _firebaseService.VerifyIdTokenAsync(request.IdToken);
                
                var company = await _companyService.CreateCompanyAsync(
                    request.CompanyName, 
                    request.Description ?? "", 
                    request.AdminEmail, 
                    firebaseUid);
                
                return Ok(new { 
                    success = true, 
                    companyId = company.Id,
                    message = "Company registered successfully" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var firebaseUid = User.FindFirst("user_id")?.Value;
                if (string.IsNullOrEmpty(firebaseUid))
                    return Unauthorized();
                
                var user = await _companyService.GetUserByFirebaseUidAsync(firebaseUid);
                if (user == null)
                    return NotFound("User not found");
                
                return Ok(new {
                    user = new {
                        id = user.Id,
                        email = user.Email,
                        role = user.Role,
                        fullName = user.FullName,
                        companyName = user.Company.Name
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
