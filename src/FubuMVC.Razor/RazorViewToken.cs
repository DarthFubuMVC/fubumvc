using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.Registration.Nodes;
using FubuMVC.Razor.RazorModel;
using RazorEngine.Templating;

namespace FubuMVC.Razor
{
    public class RazorViewToken : IViewToken
    {
        private readonly ViewDescriptor<IRazorTemplate> _descriptor;

        public RazorViewToken(ViewDescriptor<IRazorTemplate> viewDescriptor)
        {
            _descriptor = viewDescriptor;
        }

        public BehaviorNode ToBehavioralNode()
        {
            return new RazorViewNode(_descriptor);
        }

        public Type ViewType
        {
            get { return typeof(ITemplate); }
        }

        public Type ViewModelType
        {
            get { return _descriptor.ViewModel; }
        }

        public string Name
        {
            get { return _descriptor.Name(); }
        }

        public string Folder
        {
            get { return string.Empty; }
        }
        public override string ToString()
        {
            return _descriptor.RelativePath();
        }
    }
}