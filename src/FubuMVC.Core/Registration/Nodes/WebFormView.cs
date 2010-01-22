using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;

namespace FubuMVC.Core.Registration.Nodes
{
    public class WebFormView : OutputNode
    {
        private readonly string _viewName;

        public WebFormView(string viewName)
            : base(typeof (RenderFubuWebFormView))
        {
            _viewName = viewName;
        }

        public string ViewName { get { return _viewName; } }

        public override string Description { get { return "WebForm View '{0}'".ToFormat(_viewName); } }

        protected override void configureObject(ObjectDef def)
        {
            def.Child(new ViewPath
            {
                ViewName = _viewName
            });
        }
    }
}