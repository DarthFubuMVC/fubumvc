using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.UI.Tags;
using FubuMVC.StructureMap;
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
            theRequest = ElementRequest.For(new TheModel(), x => x.Name, services);
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
            services.MockFor<ITagGenerator<TheModel>>().Stub(x => x.DisplayFor(theRequest)).Return(displayTag);

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

    [TestFixture]
    public class DegradingAccessElementBuilderIntegrationTester
    {
        private ITagGenerator<TheModel> tags;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.HtmlConvention(x =>
            {
                x.Displays.Always.BuildBy(req => new HtmlTag("span").Text(req.StringValue()));
                x.Editors.Always.BuildBy(req => new TextboxTag());
                x.DegradeAccessToFields();
            });

            var container = StructureMapBootstrapper.BuildContainer(registry);
            container.Configure(x => x.For<IFieldAccessRule>().Add<TheModelFieldRules>());
            
            tags = container.GetInstance<ITagGenerator<TheModel>>();
            tags.Model = new TheModel("No", "ReadOnly", "AllRights");

        }

        private HtmlTag tagFor(Expression<Func<TheModel, object>> property)
        {
            return tags.InputFor(property);
        }

        [Test]
        public void requesting_a_tag_for_a_property_with_no_access_should_hide_the_tag()
        {
            tagFor(x => x.NoRights).Authorized().ShouldBeFalse();
        }

        [Test]
        public void requesting_a_tag_for_a_property_with_read_only_rights_should_degrade_to_the_display_tag()
        {
            var tag = tagFor(x => x.ReadOnlyRights);
            tag.TagName().ShouldEqual("span");
        }

        [Test]
        public void requesting_a_tag_for_a_property_with_full_rights_should_fall_through_to_the_standard_editor_builder_pipeline()
        {
            tagFor(x => x.AllRights).ShouldBeOfType<TextboxTag>();
        }

        public class TheModelFieldRules : IFieldAccessRule
        {
            public AccessRight RightsFor(ElementRequest request)
            {
                switch (request.Accessor.Name)
                {
                    case "NoRights":
                        return AccessRight.None;

                    case "ReadOnlyRights":
                        return AccessRight.ReadOnly;

                    case "AllRights":
                        return AccessRight.All;
                }

                throw new ArgumentOutOfRangeException();
            }

            public bool Matches(Accessor accessor)
            {
                return accessor.OwnerType == typeof (TheModel);
            }

            public FieldAccessCategory Category
            {
                get { return FieldAccessCategory.Authorization; }
            }
        }


        public class TheModel
        {
            public TheModel(string noRights, string readOnlyRights, string allRights)
            {
                NoRights = noRights;
                ReadOnlyRights = readOnlyRights;
                AllRights = allRights;
            }

            public string NoRights { get; set; }
            public string ReadOnlyRights { get; set; }
            public string AllRights { get; set; }
        }
    }
}