using System.Collections.Generic;
using System.Diagnostics;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class SettingsProviderDiagnosticsTester
    {
        private SettingsData theSettings1;
        private SettingsData theSettings2;
        private SettingsData theSettings3;

        [SetUp]
        public void SetUp()
        {
            theSettings1 = new SettingsData()
                .With("Beer", "FreeState Wheat")
                .With("Snack", "Chips");
            theSettings1.Provenance = "host";

            theSettings2 = new SettingsData()
                .With("Snack", "Peanuts")
                .With("Friend","Philip");
            theSettings2.Provenance = "other";

            theSettings3 = new SettingsData(SettingCategory.environment)
                .With("Friend","Chad");
            theSettings3.Provenance = "environment";
        }

        [Test]
        public void diagnostics_respects_order()
        {
            SettingsProvider.For(theSettings1, theSettings2).CreateDiagnosticReport()
                .Single(x => x.Key == "Snack").Value.ShouldEqual("Chips");

            SettingsProvider.For(theSettings2, theSettings1).CreateDiagnosticReport()
                .Single(x => x.Key == "Snack").Value.ShouldEqual("Peanuts");
        }

        [Test]
        public void diagnostics_gets_all_keys()
        {
            SettingsProvider.For(theSettings1, theSettings2).CreateDiagnosticReport()
                .Select(r=>r.Key)
                .ShouldHaveTheSameElementsAs("Beer", "Friend", "Snack");
        }

        [Test]
        public void diagnostics_gets_all_provenances()
        {
            SettingsProvider.For(theSettings2, theSettings1).CreateDiagnosticReport()
                .Select(r => r.Provenance).Distinct()
                .ShouldHaveTheSameElementsAs("host","other");
        }

        [Test]
        public void environment_should_override()
        {
            SettingsProvider.For(theSettings1, theSettings2, theSettings3)
                .CreateDiagnosticReport()
                .Single(x => x.Key == "Friend")
                .Value.ShouldEqual("Chad");
        }

    }
}