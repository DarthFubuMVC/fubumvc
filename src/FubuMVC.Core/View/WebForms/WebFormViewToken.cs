using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.WebForms
{
    public class WebFormViewToken : IViewToken
    {
        private readonly Type _modelType;
        private readonly Type _viewType;

        public WebFormViewToken(Type viewType)
        {
            _viewType = viewType;

            Type closedType = _viewType.FindInterfaceThatCloses(typeof (IFubuView<>));
            _modelType = closedType == null ? null : closedType.GetGenericArguments()[0];
        }

        public Type ViewType { get { return _viewType; } }

        public Type ViewModelType { get { return _modelType; } }

        public IViewToken ToViewToken()
        {
            return this;
        }

        public BehaviorNode ToBehavioralNode()
        {
            return new WebFormView(_viewType);
        }

        public string Name
        {
            get { return ViewType.Name; }
        }

        public string Folder
        {
            get { return ViewType.Namespace; } 
        }

        public override string ToString()
        {
            return ViewType.ToVirtualPath();
        }
    }
}