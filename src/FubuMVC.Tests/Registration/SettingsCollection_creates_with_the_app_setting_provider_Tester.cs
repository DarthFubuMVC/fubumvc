using System;
using FubuCore.Configuration;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class SettingsCollection_creates_with_the_app_setting_provider_Tester
    {
        [Test]
        public void pulls_value_from_config()
        {
            // look at FubuMVC.Tests.dll.config


            AppSettingsProvider.GetValueFor<DiagnosticsSettings>(x => x.TraceLevel)
                .ShouldEqual(TraceLevel.None);

            var collection = new SettingsCollection(null);
            collection.Get<DiagnosticsSettings>()
                .TraceLevel.ShouldEqual(TraceLevel.None);
        }
    }
}