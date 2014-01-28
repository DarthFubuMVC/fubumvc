using System.Collections.Specialized;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class ConnegQuerystringTester
    {
        public readonly ConnegQuerystring theParameter
            = new ConnegQuerystring("Format", "JSON", MimeType.Json);

        [Test]
        public void no_querystring()
        {
            theParameter.Determine(new NameValueCollection())
                .ShouldBeNull();
        }

        [Test]
        public void exact_hit()
        {
            var values = new NameValueCollection();
            values[theParameter.Key] = theParameter.Value;

            theParameter.Determine(values)
                .ShouldEqual(theParameter.Mimetype);
        }

        [Test]
        public void miss_on_value()
        {
            var values = new NameValueCollection();
            values[theParameter.Key] = "XML";

            theParameter.Determine(values)
                .ShouldBeNull();
        }

        [Test]
        public void case_insensitive_on_parameter()
        {
            var values = new NameValueCollection();
            values[theParameter.Key.ToLower()] = theParameter.Value;

            theParameter.Determine(values)
                .ShouldEqual(theParameter.Mimetype);
        }

        [Test]
        public void case_insensitive_on_value()
        {
            var values = new NameValueCollection();
            values[theParameter.Key] = theParameter.Value.ToLower();

            theParameter.Determine(values)
                .ShouldEqual(theParameter.Mimetype);
        }
    }
}