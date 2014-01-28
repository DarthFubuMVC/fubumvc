using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class ConnegSettingsTester
    {
        private const string theOriginalMimetype = "random/custom";

        [Test]
        public void no_correction_with_no_querystring()
        {
            var request = new StandInCurrentHttpRequest();
            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            new ConnegSettings().InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldEqual(theOriginalMimetype);
        }

        [Test]
        public void no_correction_with_wrong_querystring()
        {
            var request = new StandInCurrentHttpRequest();
            request.QueryString["Key"] = "Json";

            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            new ConnegSettings().InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldEqual(theOriginalMimetype);
        }

        [Test]
        public void correct_to_json()
        {
            var request = new StandInCurrentHttpRequest();
            request.QueryString["Format"] = "Json";

            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            new ConnegSettings().InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldEqual(MimeType.Json.Value);
        }

        [Test]
        public void correct_to_xml()
        {
            var request = new StandInCurrentHttpRequest();
            request.QueryString["Format"] = "XML";

            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            new ConnegSettings().InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldEqual(MimeType.Xml.Value);
        }

        [Test]
        public void use_a_custom_querystring_parameter()
        {
            var request = new StandInCurrentHttpRequest();
            request.QueryString["Format"] = "Text";

            var settings = new ConnegSettings();
            settings.QuerystringParameters.Add(new ConnegQuerystring("Format", "Text", MimeType.Text));

            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            settings.InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldEqual(MimeType.Text.Value);
        }

        [Test]
        public void conneg_settings_needs_to_be_application_level()
        {
            typeof(ConnegSettings).HasAttribute<ApplicationLevelAttribute>()
                .ShouldBeTrue();
        }
    }
}