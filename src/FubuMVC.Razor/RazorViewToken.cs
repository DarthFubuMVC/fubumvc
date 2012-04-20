using System;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;
using RazorEngine.Templating;

namespace FubuMVC.Razor
{
    [MarkedForTermination("use descriptor as is")]
    public class RazorViewToken : IViewToken
    {
        private readonly ViewDescriptor<IRazorTemplate> _descriptor;

        public RazorViewToken(ViewDescriptor<IRazorTemplate> viewDescriptor)
        {
            _descriptor = viewDescriptor;
        }

        public ObjectDef ToViewFactoryObjectDef()
        {
            var def = ObjectDef.ForType<ViewFactory>();
            def.DependencyByValue(_descriptor);

            return def;
        }

        public Type ViewType
        {
            get { return typeof (ITemplate); }
        }

        public Type ViewModel
        {
            get { return _descriptor.ViewModel; }
        }

        public string Name()
        {
            return _descriptor.Name();
        }

        public string Namespace
        {
            get { return string.Empty; }
        }

        public override string ToString()
        {
            return _descriptor.RelativePath();
        }
    }
}