using System.ComponentModel.DataAnnotations;

namespace QuestionBankPOC.DTOs
{
    public class AcceptInvitationRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public string IdToken { get; set; } = string.Empty;
        
        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}