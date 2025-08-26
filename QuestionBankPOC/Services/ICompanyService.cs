using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestionBankPOC.Models;

namespace QuestionBankPOC.Services
{
    public interface ICompanyService
    {
        Task<Company> CreateCompanyAsync(string name, string description, string adminEmail, string adminFirebaseUid);
        Task<Company?> GetCompanyByIdAsync(Guid companyId);
        Task<User?> GetUserByFirebaseUidAsync(string firebaseUid);
    
    }
}