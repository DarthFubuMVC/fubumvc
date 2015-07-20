using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Authentication
{
    [ApplicationLevel]
    public class AuthenticationSettings
    {
        private readonly ChainPredicate _exclusions = new ChainPredicate();
		private readonly ChainPredicate _passthroughChains = new ChainPredicate();
        private readonly AuthenticationChain _strategies;

        public AuthenticationSettings()
        {
            _exclusions.Matching<NotAuthenticatedFilter>();
			_exclusions.Matching<ExcludePassThroughAuthentication>();

	        _passthroughChains.Matching<IncludePassThroughAuthentication>();

            ExpireInMinutes = 180;
            SlidingExpiration = true;

            MaximumNumberOfFailedAttempts = 3;
            CooloffPeriodInMinutes = 60;
            MembershipEnabled = MembershipStatus.Enabled;

            _strategies = new AuthenticationChain();
        }

        public bool Enabled = false;

        /// <summary>
        ///     Register and orders the IAuthenticationStrategy's applied to this
        ///     FubuMVC application
        /// </summary>
        public AuthenticationChain Strategies
        {
            get { return _strategies; }
        }

        public ChainPredicate ExcludeChains
        {
            get { return _exclusions; }
        }

	    public ChainPredicate PassThroughChains
	    {
			get { return _passthroughChains; }
	    }

        public MembershipStatus MembershipEnabled { get; set; }
        

        public bool SlidingExpiration { get; set; }
        public int ExpireInMinutes { get; set; }

        public int MaximumNumberOfFailedAttempts { get; set; }
        public int CooloffPeriodInMinutes { get; set; }

        // *should* only be for testing
        public bool NeverExpires { get; set; }

        public bool ShouldBeExcluded(BehaviorChain chain)
        {
            return _exclusions.As<IChainFilter>().Matches(chain);
        }
    }

    public enum MembershipStatus
    {
        Enabled,
        Disabled
    }
}