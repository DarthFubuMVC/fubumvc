using System.Linq;
using FubuCore.Formatting;
using FubuMVC.Core;
using FubuMVC.RavenDb.RavenDb;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests
{
    public class loading_the_bottle_with_string_conversions
    {
        [Fact]
        public void does_not_lose_registrations_with_custom_string_display_conversions()
        {
            // This was a crazy weird bug we found at work where the Container was re-created
            // when a custom DisplayConversionRegistry was added

            /*
            using (var r = FubuRuntime.Basic())
            {
                var assembly = typeof(RavenDbRegistry).Assembly;
                r.CurrentContainer.Model.PluginTypes.Any(x => x.PluginType.Assembly == assembly)
                    .ShouldBeTrue();
            }
            */

            using (var runtime = FubuRuntime.Basic(_ => _.Policies.StringConversions<OurDisplayConventions>()))
            {
                var assembly = typeof (RavenDbRegistry).Assembly;
                runtime.CurrentContainer.Model.PluginTypes.Any(x => x.PluginType.Assembly == assembly)
                    .ShouldBeTrue();
            }

        }
    }

    public class OurDisplayConventions : DisplayConversionRegistry
    {
        public OurDisplayConventions()
        {
            /*
            IfIsType<double>().ConvertBy((req, val) =>
            {
                return req.WithFormat("{0:c}");
            });

            IfIsType<Date>().ConvertBy((req, val) =>
            {
                return "{0:MM/dd/yyyy}".ToFormat(val.Day);
            });
            */

            /*
            IfIsType<ContactPhone>().ConvertBy((req, val) =>
            {
                if (val.RawValue == null)
                    return String.Empty;

                var formattedPhone = Regex.Replace(val.RawValue, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3");

                return "{0} {1}".ToFormat(formattedPhone, val.Type);
            });

            IfIsType<SocialSecurityNumber>().ConvertBy((req, val) =>
            {
                return val != null ? val.Obfuscated() : string.Empty;
            });
            */
        }
    }
}
