using System.Collections.Immutable;

namespace IdentitySample.Authorization.ClaimBasedAuthorization.Utilities.MvcNamesUtilities
{
    public interface IMvcUtilities
    {
        public ImmutableHashSet<MvcNamesModel> MvcInfo { get; }

        public ImmutableHashSet<MvcNamesModel> MvcInfoForActionsThatRequireClaimBasedAuthorization { get; }
    }

}
