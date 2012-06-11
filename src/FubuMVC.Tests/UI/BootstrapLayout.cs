using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Tags;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.UI
{
    public class BootstrapLayout : ILabelAndFieldLayout
    {
        private HtmlTag _containingDiv;
        private HtmlTag _labelContainer;
        private HtmlTag _inputContainer;

        public BootstrapLayout()
        {
            _containingDiv = new HtmlTag("div");
            _labelContainer = new HtmlTag("div");
            _inputContainer = new HtmlTag("div");
        }

        public IEnumerable<HtmlTag> AllTags()
        {
            yield return _containingDiv;
        }

        public HtmlTag LabelTag
        {
            get { return _labelContainer.FirstChild() ?? _labelContainer; }
            set { _labelContainer.ReplaceChildren(value); }
        }

        public HtmlTag BodyTag
        {
            get { return _inputContainer.FirstChild() ?? _inputContainer; }
            set { _inputContainer.ReplaceChildren(value); }
        }

        public override string ToString()
        {
            return _containingDiv.ToString();
        }
    }

    public class OverrideFieldLayout : HtmlConventionRegistry
    {
        public OverrideFieldLayout()
        {
            Profile("DEFAULT", x =>
            {
                x.UseLabelAndFieldLayout<BootstrapLayout>();
            });
        }
    }

    [TestFixture]
    public class OverrideFieldLayoutHtmlConventionsIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x =>
            {
                x.Import<OverrideFieldLayout>();
            });
            container = new Container(x => x.For<IFubuRequest>().Singleton());

            FubuApplication.For(registry)
                .StructureMap(container)
                .Bootstrap();

            var request = container.GetInstance<IFubuRequest>();

            address = new Address();
            request.Set(address);
            request.Get<Address>().ShouldBeTheSameAs(address);

            generator = container.GetInstance<TagGenerator<Address>>();
            generator.Model = address;
        }

        #endregion

        private Address address;
        private TagGenerator<Address> generator;
        private Container container;

        [Test]
        public void field_layout_should_be_bootstrap()
        {
            var a = generator.NewFieldLayout();
            a.ShouldBeOfType<BootstrapLayout>(); //fails currently
        }
    }


}