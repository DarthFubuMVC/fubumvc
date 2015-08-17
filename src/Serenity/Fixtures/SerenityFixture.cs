using System;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authorization;
using StoryTeller;

namespace Serenity.Fixtures
{
    public class SerenityFixture : Fixture
    {
        protected void DisableAllSecurity()
        {
            var settings = Runtime.Get<SecuritySettings>();
            settings.AuthenticationEnabled = settings.AuthorizationEnabled = false;
        }

        protected FubuRuntime Runtime
        {
            get
            {
                if (Context == null) throw new InvalidOperationException("The Runtime is only available during specification execution");

                return Retrieve<FubuRuntime>();
            }
        }         
    }
}