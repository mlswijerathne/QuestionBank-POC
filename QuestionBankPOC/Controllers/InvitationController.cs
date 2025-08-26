using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuestionBankPOC.Services;
using QuestionBankPOC.DTOs;

namespace QuestionBankPOC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;
        private readonly IFirebaseService _firebaseService;
        
        public InvitationController(IInvitationService invitationService, IFirebaseService firebaseService)
        {
            _invitationService = invitationService;
            _firebaseService = firebaseService;
        }
        
        [HttpPost("create")]
        [Authorize(Policy = "EvaluatorOrAdmin")]
        public async Task<IActionResult> CreateInvitation([FromBody] CreateInvitationRequest request)
        {
            try
            {
                var companyIdClaim = User.FindFirst("companyId")?.Value;
                if (!Guid.TryParse(companyIdClaim, out var companyId))
                    return Unauthorized("Invalid company context");
                
                var invitation = await _invitationService.CreateInvitationAsync(
                    companyId, 
                    request.Email, 
                    request.Role);
                
                return Ok(new { 
                    success = true, 
                    invitationToken = invitation.Token,
                    message = "Invitation created successfully" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        [HttpGet("verify/{token}")]
        public async Task<IActionResult> VerifyInvitation(string token)
        {
            try
            {
                var invitation = await _invitationService.GetInvitationByTokenAsync(token);
                if (invitation == null)
                    return NotFound("Invalid or expired invitation");
                
                return Ok(new {
                    valid = true,
                    companyName = invitation.Company.Name,
                    role = invitation.Role,
                    email = invitation.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationRequest request)
        {
            try
            {
                // Verify Firebase token
                var firebaseUid = await _firebaseService.VerifyIdTokenAsync(request.IdToken);
                
                var user = await _invitationService.AcceptInvitationAsync(
                    request.Token, 
                    firebaseUid, 
                    request.FullName);
                
                return Ok(new { 
                    success = true, 
                    message = "Invitation accepted successfully",
                    user = new {
                        role = user.Role,
                        companyId = user.CompanyId
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
