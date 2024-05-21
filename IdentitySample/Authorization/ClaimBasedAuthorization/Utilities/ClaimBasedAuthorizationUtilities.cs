using System.Net.Http;
using System;

namespace IdentitySample.Authorization.ClaimBasedAuthorization.Utilities
{
    public class ClaimBasedAuthorizationUtilities : IClaimBasedAuthorizationUtilities
    {
        public string GetClaimToAuthorize(HttpContent httpContent)
        {
            throw new NotImplementedException();
        }
    }

}
