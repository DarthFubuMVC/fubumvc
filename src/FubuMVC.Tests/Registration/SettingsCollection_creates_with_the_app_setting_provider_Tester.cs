﻿using System;
using FubuCore.Configuration;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration
{
    
    public class SettingsCollection_creates_with_the_app_setting_provider_Tester
    {
        [Fact]
        public void pulls_value_from_config()
        {
            // look at FubuMVC.Tests.dll.config


            AppSettingsProvider.GetValueFor<DiagnosticsSettings>(x => x.TraceLevel)
                .ShouldBe(TraceLevel.None.ToString());

            var collection = new SettingsCollection();
            collection.Get<DiagnosticsSettings>()
                .TraceLevel.ShouldBe(TraceLevel.None);
        }
    }
}