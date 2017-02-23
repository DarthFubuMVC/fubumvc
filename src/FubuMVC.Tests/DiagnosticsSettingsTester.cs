﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authorization;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class DiagnosticsSettingsTester
    {
        [Fact]
        public void default_request_count_is_1000()
        {
            new DiagnosticsSettings()
                .MaxRequests.ShouldBe(1000);
        }

        [Fact]
        public void add_role()
        {
            var settings = new DiagnosticsSettings();
            settings.RestrictToRole("admin");

            settings.AuthorizationRights.Single().ShouldBeOfType<AllowRole>()
                .Role.ShouldBe("admin");
        }

        [Fact]
        public void the_default_trace_level_is_verbose()
        {
            new DiagnosticsSettings()
                .TraceLevel.ShouldBe(TraceLevel.None);
        }

        [Fact]
        public void can_override()
        {
            var settings = new DiagnosticsSettings();
            settings.SetIfNone(TraceLevel.Verbose);

            settings.TraceLevel.ShouldBe(TraceLevel.Verbose);

            settings.SetIfNone(TraceLevel.Production);

            settings.TraceLevel.ShouldBe(TraceLevel.Verbose);
        }

        [Fact]
        public void level_is_verbose_in_development()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Mode = "development"))
            {
                runtime.Get<DiagnosticsSettings>()
                    .TraceLevel.ShouldBe(TraceLevel.Verbose);
            }
        }

        [Fact]
        public void if_disabled_just_return_inner_app_func()
        {
            Func<IDictionary<string, object>, Task> inner = d => Task.Factory.StartNew(() => { });

            var settings = new DiagnosticsSettings {TraceLevel = TraceLevel.None};
            settings.WrapAppFunc(null, inner).ShouldBeSameAs(inner);
        }
    }
}