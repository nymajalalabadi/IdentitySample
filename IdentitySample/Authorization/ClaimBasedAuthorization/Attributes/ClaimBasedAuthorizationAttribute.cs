using Microsoft.AspNetCore.Authorization;
using System;

namespace IdentitySample.Authorization.ClaimBasedAuthorization.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ClaimBasedAuthorizationAttribute : AuthorizeAttribute
    {
        public ClaimBasedAuthorizationAttribute(string claimToAuthorize) : base("a")
        {
            ClaimToAuthorize = claimToAuthorize;
        }

        public string ClaimToAuthorize { get; }
    }

}
