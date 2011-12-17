using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Razor.Registration.Nodes;
using FubuMVC.Razor.RazorModel;
using ITemplate = RazorEngine.Templating.ITemplate;

namespace FubuMVC.Razor
{
    public class RazorViewToken : IViewToken
    {
        private readonly ViewDescriptor _descriptor;

        public RazorViewToken(ViewDescriptor viewDescriptor)
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
            get { return _descriptor.Namespace; }
        }
        public override string ToString()
        {
            return _descriptor.RelativePath();
        }
    }
}