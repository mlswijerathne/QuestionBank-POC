using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBankPOC.Models
{
    public class Invitation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid CompanyId { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = string.Empty;
        
        [Required]
        public string Token { get; set; } = string.Empty;
        
        public bool IsUsed { get; set; } = false;
        
        public DateTime ExpiresAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties  
        public Company Company { get; set; } = null!;
    }
}