using FubuMVC.Core.Http.AspNet;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.AspNet
{

    [TestFixture]
    public class AspNetCurrentHttpRequestTester : InteractionContext<AspNetCurrentHttpRequest>
    {
        [Test]
        public void to_full_url_should_pass_through_a_well_formed_relative_url()
        {
            
        }
    }
}