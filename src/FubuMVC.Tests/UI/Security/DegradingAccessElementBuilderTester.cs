using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Security;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Security
{
    [TestFixture]
    public class DegradingAccessElementBuilderTester
    {
        private SelfMockingServiceLocator services;
        private ElementRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            services = new SelfMockingServiceLocator();
            services.Stub<ITypeResolver>(new TypeResolver());

            var accessor = ReflectionHelper.GetAccessor<TheModel>(x => x.Name);

            theRequest = MockRepository.GeneratePartialMock<ElementRequest>(accessor);
            theRequest.Model = new TheModel();

            theRequest.Attach(services);
        }

        private AccessRight theRightsAre
        {
            set
            {
                services.MockFor<IFieldAccessService>().Stub(x => x.RightsFor(theRequest)).Return(value);
            }
        }

        private HtmlTag theResultingTag
        {
            get
            {
                var builder = new DegradingAccessElementBuilder();
                return builder.Build(theRequest);
            }
        }

        [Test]
        public void return_an_element_that_is_not_authorized_if_there_are_not_rights()
        {
            theRightsAre = AccessRight.None;
            theResultingTag.Authorized().ShouldBeFalse();
        }

        [Test]
        public void when_the_user_has_only_read_access_the_builder_should_return_the_display_for_element()
        {
            theRightsAre = AccessRight.ReadOnly;
            var displayTag = new HtmlTag("span");

            theRequest.Stub(x => x.BuildForCategory(ElementConstants.Display)).Return(displayTag);

            theResultingTag.ShouldBeTheSameAs(displayTag);
        }

        [Test]
        public void when_the_user_has_full_rights_return_null_so_that_the_construction_can_continue_to_the_next_in_the_chain()
        {
            theRightsAre = AccessRight.All;
            theResultingTag.ShouldBeNull();
        }
    
    
    
        public class TheModel
        {
            public string Name { get; set; }
        }
    }
}