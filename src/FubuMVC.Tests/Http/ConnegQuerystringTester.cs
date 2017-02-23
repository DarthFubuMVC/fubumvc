using System.Collections.Specialized;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http
{
    
    public class ConnegQuerystringTester
    {
        public readonly ConnegQuerystring theParameter
            = new ConnegQuerystring("Format", "JSON", MimeType.Json);

        [Fact]
        public void no_querystring()
        {
            theParameter.Determine(new NameValueCollection())
                .ShouldBeNull();
        }

        [Fact]
        public void exact_hit()
        {
            var values = new NameValueCollection();
            values[theParameter.Key] = theParameter.Value;

            theParameter.Determine(values)
                .ShouldBe(theParameter.Mimetype);
        }

        [Fact]
        public void miss_on_value()
        {
            var values = new NameValueCollection();
            values[theParameter.Key] = "XML";

            theParameter.Determine(values)
                .ShouldBeNull();
        }

        [Fact]
        public void case_insensitive_on_parameter()
        {
            var values = new NameValueCollection();
            values[theParameter.Key.ToLower()] = theParameter.Value;

            theParameter.Determine(values)
                .ShouldBe(theParameter.Mimetype);
        }

        [Fact]
        public void case_insensitive_on_value()
        {
            var values = new NameValueCollection();
            values[theParameter.Key] = theParameter.Value.ToLower();

            theParameter.Determine(values)
                .ShouldBe(theParameter.Mimetype);
        }
    }
}