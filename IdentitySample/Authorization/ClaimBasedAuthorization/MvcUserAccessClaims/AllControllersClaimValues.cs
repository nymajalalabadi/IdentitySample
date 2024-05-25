using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IdentitySample.Authorization.ClaimBasedAuthorization.MvcUserAccessClaims
{
    public static class AllControllersClaimValues
    {
        static AllControllersClaimValues()
        {
            var allClaimValues = new List<(string claimValueEnglish, string claimValuePersian)>();

            allClaimValues.AddRange(EmployeeControllerClaimValues.AllClaimValues);

            AllClaimValues = allClaimValues.AsReadOnly();
        }

        public static readonly ReadOnlyCollection<(string claimValueEnglish, string claimValuePersian)> AllClaimValues;
    }

}
