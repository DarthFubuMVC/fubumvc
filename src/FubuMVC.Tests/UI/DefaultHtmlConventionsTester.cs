using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Routing;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using FubuMVC.StructureMap;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class DefaultHtmlConventionsTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        private ElementRequest For(Expression<Func<AddressViewModel, object>> expression)
        {
            return new ElementRequest(new AddressViewModel(), expression.ToAccessor(), null);
        }

        [Test]
        public void add_element_name_to_select()
        {
            var select = new SelectTag();

            ElementRequest request = For(x => x.Address.City);
            request.ElementId = "AddressCity";

            DefaultHtmlConventions.AddElementName(request, select);

            select.Attr("name").ShouldEqual("AddressCity");
        }

        [Test]
        public void add_element_name_to_textbox()
        {
            var tag = new TextboxTag();
            ElementRequest request = For(x => x.Address.City);
            request.ElementId = "AddressCity";

            DefaultHtmlConventions.AddElementName(request, tag);

            tag.Attr("name").ShouldEqual("AddressCity");
        }

        [Test]
        public void do_not_overwrite_name_on_textbox_that_already_has_a_name()
        {
            var tag = new TextboxTag();
            tag.Attr("name", "ExistingName");
            var request = For(x => x.Address.City);
            request.ElementId = "AddressCity";

            DefaultHtmlConventions.AddElementName(request, tag);

            tag.Attr("name").ShouldEqual("ExistingName");
        }

        [Test]
        public void do_not_add_element_name_to_span()
        {
            var span = new HtmlTag("span");

            ElementRequest request = For(x => x.Address.City);
            request.ElementId = "AddressCity";

            DefaultHtmlConventions.AddElementName(request, span);

            span.HasAttr("name").ShouldBeFalse();
        }
    }


    [TestFixture]
    public class when_generating_an_input_for_a_string_property : using_the_default_html_conventions
    {
        protected override HtmlTag createTag()
        {
            return Generator.InputFor(x => x.Address1);
        }

        [Test]
        public void should_create_a_text_input_tag()
        {
            Tag.ShouldHaveTagName("input");
            Tag.ShouldHaveAttribute("type", "text");
        }

        [Test]
        public void should_name_the_tag_using_the_property_name()
        {
            Tag.ShouldHaveAttribute("name", "Address1");
        }

        [Test]
        public void should_populate_the_input_with_the_current_value()
        {
            Tag.ShouldHaveAttribute("value", Model.Address1);
        }
    }

    [TestFixture]
    public class when_generating_a_display_for_a_string_property : using_the_default_html_conventions
    {
        protected override HtmlTag createTag()
        {
            return Generator.DisplayFor(x => x.Address1);
        }

        [Test]
        public void should_create_a_span()
        {
            Tag.ShouldHaveTagName("span");
        }

        [Test]
        public void should_display_the_current_value()
        {
            Tag.Text().ShouldEqual(Model.Address1);
        }
    }


    [TestFixture]
    public class when_generating_a_label_for_a_deep_property : using_the_default_html_conventions
    {
        protected override HtmlTag createTag()
        {
            return Generator.LabelFor(x => x.DateEntered.Month);
        }

        [Test]
        public void should_create_a_label_tag()
        {
            Tag.ShouldHaveTagName("label");
        }

        [Test]
        public void should_display_the_name_of_the_last_property_in_the_chain()
        {
            Tag.Text().ShouldEqual("Month");
        }
    }

    [TestFixture]
    public class when_generating_a_label_for_a_camel_case_property : using_the_default_html_conventions
    {
        protected override HtmlTag createTag()
        {
            return Generator.LabelFor(x => x.DateEntered);
        }

        [Test]
        public void should_display_the_name_of_the_property_as_words_broken_up_by_casing()
        {
            Tag.Text().ShouldEqual("Date Entered");
        }
    }

    [TestFixture]
    public class BreakUpCamelCaseTester
    {
        [Test]
        public void should_consider_case_changes_as_word_boundaries()
        {
            DefaultHtmlConventions.BreakUpCamelCase("DateEntered").ShouldEqual("Date Entered");
        }

        [Test]
        public void should_consider_numbers_as_word_boundaries()
        {
            DefaultHtmlConventions.BreakUpCamelCase("The1Day2").ShouldEqual("The 1 Day 2");
        }

        [Test]
        public void should_not_consider_consecutive_numbers_as_word_boundaries()
        {
            DefaultHtmlConventions.BreakUpCamelCase("Address22").ShouldEqual("Address 22");
        }

        [Test]
        public void should_not_consider_consecutive_numbers_between_words_as_word_boundaries_()
        {
            DefaultHtmlConventions.BreakUpCamelCase("Address223City").ShouldEqual("Address 223 City");
        }

        [Test]
        public void should_consider_underscores_as_word_boundaries()
        {
            DefaultHtmlConventions.BreakUpCamelCase("Date_Entered").ShouldEqual("Date Entered");
        }
    }

    public class using_the_default_html_conventions
    {
        protected TagGenerator<Address> Generator;
        protected HtmlTag Tag;
        protected Address Model;

        [SetUp]
        public void Setup()
        {
            var registry = new FubuRegistry(x =>
            {
                
                x.HtmlConvention<DefaultHtmlConventions>();
            });
            var container = new Container(x => x.For<IFubuRequest>().Singleton());
            var facility = new StructureMapContainerFacility(container);

            new FubuBootstrapper(facility, registry).Bootstrap(new List<RouteBase>());
            Model = new Address
            {
                Address1 = "123 Main St.",
                DateEntered = new DateTime(2010, 5, 25, 11, 30, 0),
                Order = 42
            };
            container.GetInstance<IFubuRequest>().Set(Model);

            Generator = container.GetInstance<TagGenerator<Address>>();
            Generator.Model = Model;
            Tag = createTag();
        }

        protected virtual HtmlTag createTag()
        {
            return null;
        }
    }
}