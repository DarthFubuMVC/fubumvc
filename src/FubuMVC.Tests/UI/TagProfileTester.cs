using System;
using System.Collections.Generic;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Tags;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class TagProfileTester
    {
        [Test]
        public void by_default_the_label_and_field_layout_is_definition_list()
        {
            var profile = new TagProfile("default");
            profile.NewLabelAndFieldLayout().ShouldBeOfType<DefinitionListLabelAndField>();
        }

        [Test]
        public void override_the_label_and_field_layout()
        {
            var profile = new TagProfile("default");
            profile.UseLabelAndFieldLayout<FakeLabelAndField>();
            profile.NewLabelAndFieldLayout().ShouldBeOfType<FakeLabelAndField>();
        }


        [Test]
        public void override_the_label_and_field_layout_with_a_func()
        {
            var profile = new TagProfile("default");
            profile.UseLabelAndFieldLayout(() => new FakeLabelAndField());
            profile.NewLabelAndFieldLayout().ShouldBeOfType<FakeLabelAndField>();
        }
    }

    public class FakeLabelAndField : ILabelAndFieldLayout
    {
        public IEnumerable<HtmlTag> AllTags()
        {
            throw new NotImplementedException();
        }

        public HtmlTag LabelTag
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public HtmlTag BodyTag
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}