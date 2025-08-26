using System.ComponentModel.DataAnnotations;

namespace QuestionBankPOC.DTOs
{
    public class CreateInvitationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [RegularExpression("^(evaluator|candidate)$")]
        public string Role { get; set; } = string.Empty;
    }
}