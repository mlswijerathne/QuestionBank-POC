using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBankPOC.Services
{
    public interface IFirebaseService
    {
        Task SetCustomClaimsAsync(string firebaseUid, Dictionary<string, object> claims);
        Task<string> VerifyIdTokenAsync(string idToken);
        Task<Dictionary<string, object>?> GetUserClaimsAsync(string firebaseUid);
    }
    
}