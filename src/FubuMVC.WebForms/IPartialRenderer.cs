using System;
using System.Globalization;
using System.IO;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;

namespace FubuMVC.WebForms
{
    public interface IPartialRenderer
    {
        IFubuPage CreateControl<T>() where T : IFubuPage;
        IFubuPage CreateControl(Type controlType);

        string Render<T>(IFubuPage view, T viewModel, string prefix, int? index = null) where T : class;
        void Render<T>(IFubuPage view, T viewModel, string prefix, TextWriter writer, int? index = null) where T : class;

        string Render<T>(IFubuPage parentPage, IFubuPage partialControl, T viewModel, string prefix) where T : class;
        string Render<T>(IFubuPage view, Type controlType, T viewModel, string prefix) where T : class;
        void Render<T>(IFubuPage view, Type controlType, T viewModel, string prefix, TextWriter writer) where T : class;
    }

    public class PartialRenderer : IPartialRenderer
    {
        private readonly IWebFormsControlBuilder _builder;
        private readonly IPageActivator _activator;
        private readonly IFubuRequest _request;

        public PartialRenderer(IWebFormsControlBuilder builder, IPageActivator activator, IFubuRequest request)
        {
            _builder = builder;
            _activator = activator;
            _request = request;
        }

        public IFubuPage CreateControl<VIEW>() where VIEW : IFubuPage
        {
            return CreateControl(typeof(VIEW));
        }

        public IFubuPage CreateControl(Type controlType)
        {
            //TODO: I'm not sure this IF is required any more, I think that's what the
            // second arg to LoadControlFromVirtualPath does. This needs some investigation before
            // deleting.
            if (!typeof(IFubuPage).IsAssignableFrom(controlType) || !typeof(Control).IsAssignableFrom(controlType))
            {
                throw new InvalidOperationException(String.Format(
                                                        "PartialRenderer cannot render type '{0}'. It can only render System.Web.UI.Control objects which implement the IFubuPage interface.",
                                                        (controlType == null) ? "(null)" : controlType.Name));
            }

            var virtualPath = controlType.ToVirtualPath();
            var control = _builder.LoadControlFromVirtualPath(virtualPath, controlType);
            return control as IFubuPage;
        }

        public string Render<T>(IFubuPage view, T viewModel, string prefix, int? index = null) where T : class
        {
            var writer = new StringWriter(CultureInfo.CurrentCulture);
            Render(view, viewModel, prefix, writer, index);
            return writer.GetStringBuilder().ToString();
        }

        public void Render<T>(IFubuPage view, T viewModel, string prefix, TextWriter writer, int? index = null) where T : class
        {
            var page = new Page();
            page.Controls.Add(view as Control);

            var shouldClearModel = false;
            if (viewModel != null)
            {
                shouldClearModel = !_request.Has(viewModel.GetType());
                _request.Set(viewModel.GetType(), viewModel);
            }
            _activator.Activate(view);

            setParentPageIfNotAlreadySet(view, page);

            if (index.HasValue)
            {
                prefix = "{0}[{1}]".ToFormat(prefix, index);
            }

            view.ElementPrefix = prefix;

            _builder.ExecuteControl(page, writer);

            writer.Flush();

            if (shouldClearModel)
            {
                _request.Clear(viewModel.GetType());
            }
        }

        public string Render<T>(IFubuPage parentView, Type controlType, T viewModel, string prefix) where T : class
        {
            var view = CreateControl(controlType);

            setParentPageIfNotAlreadySet(view, (Control)parentView);

            return Render(view, viewModel, prefix);
        }

        public string Render<T>(IFubuPage parentPage, IFubuPage partialControl, T viewModel, string prefix) where T : class
        {
            setParentPageIfNotAlreadySet(partialControl, (Control)parentPage);

            return Render(partialControl, viewModel, prefix);
        }

        public void Render<T>(IFubuPage parentView, Type controlType, T viewModel, string prefix, TextWriter writer) where T : class
        {
            var view = CreateControl(controlType);

            setParentPageIfNotAlreadySet(view, (Control)parentView);

            Render(view, viewModel, prefix, writer);
        }

        private static void setParentPageIfNotAlreadySet(IFubuPage view, Control parent)
        {
            var controlThatNeedsParent = view as INeedToKnowAboutParentPage;
            if (controlThatNeedsParent == null) return;
            if (controlThatNeedsParent.ParentPage != null) return;

            controlThatNeedsParent.ParentPage = parent as Page ?? parent.Page;
        }
    }
}