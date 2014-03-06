using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.Registration;
using FubuMVC.Razor.Rendering;

namespace FubuMVC.Razor.RazorModel
{
    public class RazorTemplate : Template
    {
        private readonly ITemplateFactory _factory;
        private static readonly IViewParser ViewParser = new ViewParser();
        private readonly Guid _generatedViewId = Guid.NewGuid();

        public RazorTemplate(IFubuFile file, ITemplateFactory factory) : base(file)
        {
            _factory = factory;
        }

        public Guid GeneratedViewId
        {
            get { return _generatedViewId; }
        }


        // TODO -- some commonality here between RazorTemplate and SparkTemplate!
        protected override Parsing createParsing()
        {
            var chunks = ViewParser.Parse(FilePath).ToList();

            return new Parsing
            {
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces()
            };
        }

        public override IRenderableView GetView()
        {
            return CreateInstance();
        }

        public override IRenderableView GetPartialView()
        {
            return CreateInstance(true);
        }

        private IFubuRazorView CreateInstance(bool partialOnly = false)
        {
            var currentDescriptor = this;
            var returnTemplate = _factory.GetView(currentDescriptor);
            returnTemplate.OriginTemplate = this;
            var currentTemplate = returnTemplate;

            while (currentDescriptor.Master != null && !partialOnly)
            {
                currentDescriptor = currentDescriptor.Master.As<RazorTemplate>();

                var layoutTemplate = _factory.GetView(currentDescriptor);
                layoutTemplate.OriginTemplate = returnTemplate.OriginTemplate;
                currentTemplate.UseLayout(layoutTemplate);
                currentTemplate = layoutTemplate;
            }

            return returnTemplate;
        }
    }
}