using System;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Security;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Security
{
    [TestFixture, Ignore("Just for 9/11/2012")]
    public class DegradingAccessElementBuilderIntegrationTester
    {
        //private ITagGenerator<TheModel> tags;

        //[SetUp]
        //public void SetUp()
        //{
        //    var registry = new FubuRegistry();
        //    registry.Import<HtmlConventionRegistry>(x =>
        //    {
        //        x.Displays.Always.BuildBy(req => new HtmlTag("span").Text(req.StringValue()));
        //        x.Editors.Always.BuildBy(req => new TextboxTag());
        //        x.DegradeAccessToFields();
        //    });


        //    var container = new Container();
        //    container.Configure(x => x.For<IFieldAccessRule>().Add<TheModelFieldRules>());
        //    FubuApplication.For(() => registry).StructureMap(container).Bootstrap();

        //    tags = container.GetInstance<ITagGenerator<TheModel>>();
        //    tags.Model = new TheModel("No", "ReadOnly", "AllRights");

        //}

        //private HtmlTag tagFor(Expression<Func<TheModel, object>> property)
        //{
        //    return tags.InputFor(property);
        //}

        //[Test]
        //public void requesting_a_tag_for_a_property_with_no_access_should_hide_the_tag()
        //{
        //    tagFor(x => x.NoRights).Authorized().ShouldBeFalse();
        //}

        //[Test]
        //public void requesting_a_tag_for_a_property_with_read_only_rights_should_degrade_to_the_display_tag()
        //{
        //    var tag = tagFor(x => x.ReadOnlyRights);
        //    tag.TagName().ShouldEqual("span");
        //}

        //[Test]
        //public void requesting_a_tag_for_a_property_with_full_rights_should_fall_through_to_the_standard_editor_builder_pipeline()
        //{
        //    tagFor(x => x.AllRights).ShouldBeOfType<TextboxTag>();
        //}

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