using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using System.Linq;

namespace FubuMVC.Core.Registration.Nodes
{
    public class WebFormView : OutputNode, IMayHaveInputType
    {
        private readonly string _viewName;

        public WebFormView(string viewName)
            : base(typeof (RenderFubuWebFormView))
        {
            _viewName = viewName;
        }

        // TODO -- make this strategy of resolving views swappable!
        public WebFormView(Type viewType) : this(viewType.ToVirtualPath())
        {
            var pageInterface = viewType.FindInterfaceThatCloses(typeof (IFubuPage<>));
            if (pageInterface != null) InputType = pageInterface.GetGenericArguments().Single();
        }

        public Type InputType { get; set; }

        public string ViewName { get { return _viewName; } }

        public override string Description { get { return "WebForm View '{0}'".ToFormat(_viewName); } }

        protected override void configureObject(ObjectDef def)
        {
            def.Child(new ViewPath
            {
                ViewName = _viewName
            });
        }

        Type IMayHaveInputType.InputType()
        {
            return InputType;
        }
    }
}