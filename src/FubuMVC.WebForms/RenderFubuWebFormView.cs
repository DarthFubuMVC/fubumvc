using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;

namespace FubuMVC.WebForms
{
    public class RenderFubuWebFormView : BasicBehavior
    {
        private readonly ViewPath _path;
        private readonly IWebFormsControlBuilder _builder;
        private readonly IWebFormRenderer _renderer;
        private readonly IPageActivator _activator;

        public RenderFubuWebFormView(ViewPath path, IWebFormsControlBuilder builder, IWebFormRenderer renderer, IPageActivator activator)
            : base(PartialBehavior.Executes)
        {
            _path = path;
            _builder = builder;
            _renderer = renderer;
            _activator = activator;
        }

        public ViewPath View { get { return _path; } }

        protected override DoNext performInvoke()
        {
            var control = _builder.LoadControlFromVirtualPath(_path.ViewName, typeof(IFubuPage));

            _activator.Activate((IFubuPage) control);

            _renderer.RenderControl(control);

            return DoNext.Continue;
        }
    }
}