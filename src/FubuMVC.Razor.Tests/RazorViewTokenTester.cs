using System;
using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests
{
    [TestFixture]
    public class RazorViewTokenTester
    {
        private RazorViewToken _token;
        private IRazorTemplate _template;
        private ViewDescriptor<IRazorTemplate> _descriptor;

        [SetUp]
        public void SetUp()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            _template = new Template(Path.Combine(root, "Views", "Home", "Home.cshtml"), root, TemplateConstants.HostOrigin);
            
            _descriptor = new ViewDescriptor<IRazorTemplate>(_template)
            {
                ViewModel = typeof (ProductModel)
            };
            
            _template.Descriptor = _descriptor;

            _token = new RazorViewToken(_template.Descriptor.As<ViewDescriptor<IRazorTemplate>>());
        }

        [Test]
        public void name_is_item_name()
        {
            _token.Name().ShouldEqual(_template.Name());
        }

        [Test]
        public void view_model_type_is_descriptor_view_model()
        {
            _token.ViewModel.ShouldEqual(_descriptor.ViewModel);
        }



    }

    public class ProductModel
    {
    }
}