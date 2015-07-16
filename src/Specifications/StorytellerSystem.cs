using FubuCore.Binding;
using FubuMVC.Authentication;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.StructureMap;
using FubuMVC.PersistedMembership;
using Serenity;
using StructureMap;

namespace Specifications
{
    public class AuthenticationStorytellerApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<StorytellerFubuRegistry>();
        }
    }

    public class StorytellerFubuRegistry : FubuRegistry
    {
        public StorytellerFubuRegistry()
        {
            Import<PersistedMembership<User>>();

            AlterSettings<AuthenticationSettings>(x =>
            {
                x.ExcludeChains.ChainMatches(c => c is DiagnosticChain);
            });
        }
    }

    public class SpecificationSystem : FubuMvcSystem<AuthenticationStorytellerApplication> { }

}