using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.ViewEngine
{
    public class HtmlDocumentViewToken : IViewToken
    {
        private readonly Type _documentType;
        private readonly Type _viewModel;

        public HtmlDocumentViewToken(Type documentType)
        {
            _viewModel = documentType.FindInterfaceThatCloses(typeof (IFubuPage<>)).GetGenericArguments().Single();
            _documentType = documentType;
        }

        public Type ViewType
        {
            get { return _documentType; }
        }

        public Type ViewModel
        {
            get { return _viewModel; }
        }

        public string Name()
        {
            return _documentType.Name;
        }

        public string Namespace
        {
            get { return _documentType.Namespace; }
        }

        public ObjectDef ToViewFactoryObjectDef()
        {
            return new ObjectDef(typeof (HtmlDocumentViewFactory<>), _documentType);
        }

        public string ProfileName { get; set; }

        public override string ToString()
        {
            return string.Format("HtmlDocument: {0}, Profile: {1}", _documentType, ProfileName);
        }
    }
}