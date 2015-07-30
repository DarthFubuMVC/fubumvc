using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.RavenDb.Membership;
using Serenity;

namespace Specifications
{
    public class AuthenticationStorytellerApplication : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory)
        {
            return FubuApplication.For<StorytellerFubuRegistry>();
        }
    }

    public class StorytellerFubuRegistry : FubuRegistry
    {
        public StorytellerFubuRegistry()
        {
            Import<PersistedMembership<User>>();

            AlterSettings<AuthenticationSettings>(x => x.ExcludeChains.ChainMatches(c => c is DiagnosticChain));
        }
    }

    public class SpecificationSystem : FubuMvcSystem<AuthenticationStorytellerApplication>
    {
    }
}