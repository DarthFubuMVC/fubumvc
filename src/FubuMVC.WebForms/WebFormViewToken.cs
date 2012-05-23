using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;

namespace FubuMVC.WebForms
{
    public class WebFormViewToken : IViewToken
    {
        private readonly Type _model;
        private readonly Type _viewType;

        public WebFormViewToken(Type viewType)
        {
            _viewType = viewType;

            Type closedType = _viewType.FindInterfaceThatCloses(typeof (IFubuPage<>));
            _model = closedType == null ? null : closedType.GetGenericArguments()[0];
        }

        public Type ViewType { get { return _viewType; } }

        public Type ViewModel { get { return _model; } }

        public IViewToken ToViewToken()
        {
            return this;
        }

        public ObjectDef ToViewFactoryObjectDef()
        {
            return new ObjectDef(typeof(WebFormsViewFactory<>), _viewType);
        }

        public string Name()
        {
            return ViewType.Name;
        }

        public string Namespace
        {
            get { return ViewType.Namespace; } 
        }

        public override string ToString()
        {
            return ViewType.ToVirtualPath();
        }
    }
}