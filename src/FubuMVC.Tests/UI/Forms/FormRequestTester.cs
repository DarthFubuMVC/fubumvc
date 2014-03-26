using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class FormRequestTester
    {
        private InMemoryServiceLocator theServices;
        private ChainSearch theSearch;
        private FormRequest theRequest;
        private object theInput;

        private IChainResolver theResolver;
	    private IChainUrlResolver theUrlResolver;
        private RoutedChain theChain;

        [SetUp]
        public void SetUp()
        {
            theServices = new InMemoryServiceLocator();
            theSearch = ChainSearch.ByUniqueInputType(typeof (object));
            theInput = new object();

            theResolver = MockRepository.GenerateStub<IChainResolver>();
            theUrlResolver = MockRepository.GenerateStub<IChainUrlResolver>();

            theChain = new RoutedChain("something");

            theServices.Add(theResolver);
            theServices.Add(theUrlResolver);

            theRequest = new FormRequest(theSearch, theInput);
        }

        [Test]
        public void throws_if_the_chain_cannot_be_found()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => theRequest.Attach(theServices));
        }

        [Test]
        public void throws_if_the_route_is_null()
        {
            var chain = new BehaviorChain();
            theResolver.Stub(x => x.Find(theSearch)).Return(chain);

            Exception<FubuException>.ShouldBeThrownBy(() => theRequest.Attach(theServices));
        }

        [Test]
        public void sets_the_full_url()
        {
            var fullUrl = "this/is/a/test";

            var route = new RouteDefinition("blah");
            route.ApplyInputType(typeof(object));
            
            theChain = new RoutedChain(fullUrl);
            
            theUrlResolver.Stub(x => x.UrlFor(theInput, theChain)).Return(fullUrl);
            theResolver.Stub(x => x.Find(theSearch)).Return(theChain);

            theRequest.Attach(theServices);

            theRequest.Url.ShouldEqual(fullUrl);
        }

        [Test]
        public void close_tag_false_by_default()
        {
            theRequest.CloseTag.ShouldBeFalse();
        }

        [Test]
        public void close_tag_true_when_set()
        {
            new FormRequest(theSearch, theInput, true).CloseTag.ShouldBeTrue();
        }
    }
}