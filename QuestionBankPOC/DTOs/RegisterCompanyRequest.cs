using System.ComponentModel.DataAnnotations;

namespace QuestionBankPOC.DTOs
{
    public class RegisterCompanyRequest
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; } = string.Empty;
        
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}