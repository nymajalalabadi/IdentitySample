using System.Net.Http;

namespace IdentitySample.Authorization.ClaimBasedAuthorization.Utilities
{
    public interface IClaimBasedAuthorizationUtilities
    {
        string GetClaimToAuthorize(HttpContent httpContent);
    }
}
