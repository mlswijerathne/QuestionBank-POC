using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBankPOC.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string FirebaseUid { get; set; } = string.Empty;
        
        public Guid CompanyId { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = string.Empty; // admin, evaluator, candidate
        
        public string? FullName { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Company Company { get; set; } = null!;
    }
}