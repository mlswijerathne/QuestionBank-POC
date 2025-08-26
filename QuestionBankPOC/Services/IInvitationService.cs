using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestionBankPOC.Models;

namespace QuestionBankPOC.Services
{
    public interface IInvitationService
    {
        Task<Invitation> CreateInvitationAsync(Guid companyId, string email, string role);
        Task<Invitation?> GetInvitationByTokenAsync(string token);
        Task<User> AcceptInvitationAsync(string token, string firebaseUid, string fullName);
    }
}