using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class TagFactoryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            TagBuilder textboxes = request => new TextboxTag().Attr("value", request.RawValue);
            TagBuilder checkboxes = request => new CheckboxTag(request.Value<bool>());

            TagModifier setName = (def, tag) => tag.Attr("name", def.ElementId);
            TagModifier setRequired = (def, tag) => tag.AddClass("required");

            factory = new TagFactory();
            factory.AddBuilder(new LambdaElementBuilder(def => def.Is<string>(), def => textboxes));
            factory.AddBuilder(new LambdaElementBuilder(def => def.Is<bool>(), def => checkboxes));

            factory.AddModifier(new LambdaElementModifier(def => true, def => setName));
            factory.AddModifier(new LambdaElementModifier(def => def.Accessor.HasAttribute<FakeRequiredAttribute>(),
                                                          def => setRequired));

            address = new Address();
        }

        #endregion

        private TagFactory factory;
        private Address address;

        private HtmlTag For(Expression<Func<Address, object>> expression)
        {
            Accessor accessor = expression.ToAccessor();
            var request = new ElementRequest(address, accessor, null);
            request.ElementId = accessor.Name;
            return factory.Build(request);
        }

        [Test]
        public void bool_property_is_built_with_checkbox_1()
        {
            address.IsActive = true;

            HtmlTag tag = For(x => x.IsActive);

            tag.ShouldHaveTagName("input");
            tag.ShouldHaveAttribute("type", "checkbox");
            tag.ShouldHaveAttribute("checked", "true");
        }


        [Test]
        public void bool_property_is_built_with_checkbox_2()
        {
            address.IsActive = false;

            HtmlTag tag = For(x => x.IsActive);

            tag.ShouldHaveTagName("input");
            tag.ShouldHaveAttribute("type", "checkbox");
            tag.HasAttr("checked").ShouldBeFalse();
        }

        [Test]
        public void conditional_modifier_is_applied()
        {
            address.Address1 = "4 Brookhaven";
            address.Address2 = "Bentonville";

            For(x => x.Address1).ShouldHaveClass("required");
            For(x => x.Address2).ShouldNotHaveClass("required");
        }

        [Test]
        public void merge_testing()
        {
            var secondFactory = new TagFactory();
            secondFactory.AddBuilder(new LambdaElementBuilder(def => def.Is<int>(),
                                                              def => request => new TextboxTag().AddClass("integer")));
            secondFactory.AddModifier(new LambdaElementModifier(def => true, def => (r, t) => t.AddClass("editor")));

            factory.Merge(secondFactory);

            HtmlTag tag = For(x => x.Order);

            tag.ShouldHaveClass("integer");
            tag.ShouldHaveClass("editor");
        }

        [Test]
        public void string_property_is_built_with_the_textbox()
        {
            address.Address1 = "4 Brookhaven";
            HtmlTag tag = For(x => x.Address1);

            tag.ShouldHaveTagName("input");
            tag.ShouldHaveAttribute("type", "text");
            tag.ShouldHaveAttribute("value", address.Address1);
        }

        [Test]
        public void the_name_is_set()
        {
            For(x => x.IsActive).ShouldHaveAttribute("name", "IsActive");
            For(x => x.Address1).ShouldHaveAttribute("name", "Address1");
            For(x => x.City).ShouldHaveAttribute("name", "City");
        }
    }

    public class FakeRequiredAttribute : Attribute
    {
    }

    public class FakeMaximumStringLength : Attribute
    {
        private readonly int _maxLength;

        public FakeMaximumStringLength(int maxLength)
        {
            _maxLength = maxLength;
        }

        public int MaxLength { get { return _maxLength; } }
    }
}