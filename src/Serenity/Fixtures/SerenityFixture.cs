using System;
using FubuCore;
using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authorization;
using StoryTeller;

namespace Serenity.Fixtures
{
    public class SerenityFixture : Fixture
    {
        protected void DisableAllSecurity()
        {
            Runtime.Get<SecuritySettings>().Disable();
        }

        protected void EnableAllSecurity()
        {
            Runtime.Get<SecuritySettings>().Reset();
        }

        protected FubuRuntime Runtime
        {
            get
            {
                if (Context == null) throw new InvalidOperationException("The Runtime is only available during specification execution");

                return Retrieve<FubuRuntime>();
            }
        }

        protected void ResetTheClock()
        {
            Runtime.Get<IClock>().As<Clock>().Live();
        }


        protected void AdvanceTheClock(TimeSpan timespan)
        {
            var clock = Runtime.Get<IClock>().As<Clock>();
            var time = clock.UtcNow().Add(timespan).ToLocalTime();

            clock.LocalNow(time);
        }
    }
}