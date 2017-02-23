using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Http.Headers
{
    
    public class HttpHeaderValuesTester
    {
        [Fact]
        public void build_with_key_name_pair_in_ctor()
        {
            var values = new HttpHeaderValues(HttpResponseHeaders.Via, "something");

            values["Via"].ShouldBe("something");
            values.Headers.ShouldHaveTheSameElementsAs(new Header("Via", "something"));
        }
    }
}