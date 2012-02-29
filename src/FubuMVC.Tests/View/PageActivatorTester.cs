using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;

using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class PageActivatorTester 
    {
        private PageActivationRuleCache _theRuleCache;
        private IServiceLocator theServices;
        private PageActivator theActivator;
        private InMemoryFubuRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            _theRuleCache = new PageActivationRuleCache(new IPageActivationSource[0]);
            theServices = MockRepository.GenerateMock<IServiceLocator>();

            theRequest = new InMemoryFubuRequest();
            theServices.Stub(x => x.GetInstance<IFubuRequest>()).Return(theRequest);

            theActivator = new PageActivator(theServices, _theRuleCache);
        }

        [Test]
        public void should_set_the_locator_on_a_page()
        {
            var page = MockRepository.GenerateMock<IFubuPage>();

            theActivator.Activate(page);

            page.AssertWasCalled(x => x.ServiceLocator = theServices);
        }

        [Test]
        public void activate_a_fubu_page_with_a_model_on_it()
        {
            var input = new FubuPageTarget();
            theRequest.Set(input);

            var theModelPage = MockRepository.GenerateMock<IFubuPage<FubuPageTarget>>();
            
            // Gets around a defensive prog. check
            theModelPage.Stub(x => x.Model).Return(input);

            theActivator.Activate(theModelPage);

            theModelPage.AssertWasCalled(x => x.Model = input);

            
        }

        [Test]
        public void activate_a_fubu_page_with_a_profile_rule()
        {
            _theRuleCache.IfTheInputModelOfTheViewMatches(type => type == typeof(FubuPageTarget2))
                .SetTagProfileTo("the profile");

            var theModelPage = MockRepository.GenerateMock<IFubuPage<FubuPageTarget>>();
            var theModelPage2 = MockRepository.GenerateMock<IFubuPage<FubuPageTarget2>>();
            var tags = MockRepository.GenerateMock<ITagGenerator<FubuPageTarget>>();
            var tags2 = MockRepository.GenerateMock<ITagGenerator<FubuPageTarget2>>();


            theModelPage.Stub(x => x.Model).Return(new FubuPageTarget());
            theModelPage2.Stub(x => x.Model).Return(new FubuPageTarget2());
            theModelPage.Stub(x => x.Get<ITagGenerator<FubuPageTarget>>()).Return(tags);
            theModelPage2.Stub(x => x.Get<ITagGenerator<FubuPageTarget2>>()).Return(tags2);

        
            theActivator.Activate(theModelPage);
            theActivator.Activate(theModelPage2);

            tags.AssertWasNotCalled(x => x.SetProfile("the profile"), x => x.IgnoreArguments());
            tags2.AssertWasCalled(x => x.SetProfile("the profile"));
        }
    }

    public class FubuPageTarget{}
    public class FubuPageTarget2{}
    public class FubuPageTarget3{}
    public class FubuPageTarget4{}
    public class FubuPageTarget5{}
}