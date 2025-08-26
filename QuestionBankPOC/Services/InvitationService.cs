using Microsoft.EntityFrameworkCore;
using QuestionBankPOC.Data;
using QuestionBankPOC.Models;
using System.Security.Cryptography;
using System.Text;

namespace QuestionBankPOC.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFirebaseService _firebaseService;
        
        public InvitationService(ApplicationDbContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }
        
        public async Task<Invitation> CreateInvitationAsync(Guid companyId, string email, string role)
        {
            var invitation = new Invitation
            {
                CompanyId = companyId,
                Email = email,
                Role = role,
                Token = GenerateSecureToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7) // 7 days expiry
            };
            
            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();
            
            return invitation;
        }
        
        public async Task<Invitation?> GetInvitationByTokenAsync(string token)
        {
            return await _context.Invitations
                .Include(i => i.Company)
                .FirstOrDefaultAsync(i => i.Token == token && 
                                    !i.IsUsed && 
                                    i.ExpiresAt > DateTime.UtcNow);
        }
        
        public async Task<User> AcceptInvitationAsync(string token, string firebaseUid, string fullName)
        {
            var invitation = await GetInvitationByTokenAsync(token);
            if (invitation == null)
                throw new InvalidOperationException("Invalid or expired invitation");
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Create user
                var user = new User
                {
                    FirebaseUid = firebaseUid,
                    CompanyId = invitation.CompanyId,
                    Email = invitation.Email,
                    Role = invitation.Role,
                    FullName = fullName
                };
                
                _context.Users.Add(user);
                
                // Mark invitation as used
                invitation.IsUsed = true;
                
                await _context.SaveChangesAsync();
                
                // Set Firebase custom claims
                var claims = new Dictionary<string, object>
                {
                    ["role"] = user.Role,
                    ["companyId"] = user.CompanyId.ToString()
                };
                
                await _firebaseService.SetCustomClaimsAsync(firebaseUid, claims);
                
                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        private static string GenerateSecureToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[32];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}