using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.PersistedMembership;
using FubuMVC.StructureMap;
using Serenity;
using StructureMap;

namespace Specifications
{
    public class AuthenticationStorytellerApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<StorytellerFubuRegistry>().StructureMap(new Container());
        }
    }

    public class StorytellerFubuRegistry : FubuRegistry
    {
        public StorytellerFubuRegistry()
        {
            Import<PersistedMembership<User>>();
        }
    }

}