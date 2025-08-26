using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;

namespace QuestionBankPOC.Services
{
    public class FirebaseService : IFirebaseService
    {
         public async Task SetCustomClaimsAsync(string firebaseUid, Dictionary<string, object> claims)
        {
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(firebaseUid, claims);
        }
        
        public async Task<string> VerifyIdTokenAsync(string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return decodedToken.Uid;
            }
            catch
            {
                throw new UnauthorizedAccessException("Invalid Firebase token");
            }
        }
        
        public async Task<Dictionary<string, object>?> GetUserClaimsAsync(string firebaseUid)
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(firebaseUid);
            return (Dictionary<string, object>?)userRecord.CustomClaims;
        }
    
    }
}