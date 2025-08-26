using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuestionBankPOC.Data;
using QuestionBankPOC.Models;

namespace QuestionBankPOC.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFirebaseService _firebaseService;
        
        public CompanyService(ApplicationDbContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }
        
        public async Task<Company> CreateCompanyAsync(string name, string description, string adminEmail, string adminFirebaseUid)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Create company
                var company = new Company
                {
                    Name = name,
                    Description = description
                };
                
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                
                // Create admin user
                var admin = new User
                {
                    FirebaseUid = adminFirebaseUid,
                    CompanyId = company.Id,
                    Email = adminEmail,
                    Role = "admin"
                };
                
                _context.Users.Add(admin);
                await _context.SaveChangesAsync();
                
                // Set Firebase custom claims
                var claims = new Dictionary<string, object>
                {
                    ["role"] = "admin",
                    ["companyId"] = company.Id.ToString()
                };
                
                await _firebaseService.SetCustomClaimsAsync(adminFirebaseUid, claims);
                
                await transaction.CommitAsync();
                return company;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<Company?> GetCompanyByIdAsync(Guid companyId)
        {
            return await _context.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == companyId);
        }
        
        public async Task<User?> GetUserByFirebaseUidAsync(string firebaseUid)
        {
            return await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
        }
    }
}