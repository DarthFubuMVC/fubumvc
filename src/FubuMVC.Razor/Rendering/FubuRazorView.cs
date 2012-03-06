using System;
using System.Web;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using RazorEngine.Templating;
using ITemplate = RazorEngine.Templating.ITemplate;

namespace FubuMVC.Razor.Rendering
{
    public abstract class FubuRazorView : TemplateBase, IFubuRazorView
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();

        protected FubuRazorView()
        {
            _services.OnMissing = type => ServiceLocator.GetInstance(type);
        }

        public IServiceLocator ServiceLocator { get; set; }

        public T Get<T>()
        {
            return (T)_services[typeof(T)];
        }

        public T GetNew<T>()
        {
            return (T)ServiceLocator.GetInstance(typeof(T));
        }

        protected override ITemplate ResolveLayout(string name)
        {
            return Layout;
        }

        public IUrlRegistry Urls
        {
            get { return Get<IUrlRegistry>(); }
        }

        public Func<string, string> SiteResource { get; set; }

        string IFubuPage.ElementPrefix { get; set; }

        void IRenderableView.Render()
        {
            var result = ((ITemplate) this).Run(new ExecuteContext());
            Get<IOutputWriter>().WriteHtml(result);
        }

        void IFubuRazorView.RenderPartial()
        {
            _Layout = null;
            ((IFubuRazorView)this).Render();
        }

        public HtmlTag Tag(string tagName)
        {
            return new HtmlTag(tagName);
        }

        public override void Write(object value)
        {
            if(value is IHtmlString)
                base.Write(new EncodedString(value));
            else
                base.Write(value);
        }

        private ITemplate _layout;
        public ITemplate Layout
        {
            get { return _layout; }
            set
            {
                _layout = value;
                if (_Layout == null)
                    _Layout = _layout.GetType().ToString();
            }
        }
    }

    public abstract class FubuRazorView<TViewModel> : FubuRazorView, IFubuRazorView, IFubuPage<TViewModel> where TViewModel : class
    {
        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<TViewModel>();
        }

        public void SetModel(object model)
        {
            SetModel(model as TViewModel);
        }

        public void SetModel(TViewModel model)
        {
            Model = model;
        }

        void IRenderableView.Render()
        {
            SetModel(ServiceLocator.GetInstance<IFubuRequest>());
            var result = ((ITemplate) this).Run(new ExecuteContext());
            Get<IOutputWriter>().WriteHtml(result);
        }

        void IFubuRazorView.RenderPartial()
        {
            _Layout = null;
            ((IFubuRazorView)this).Render();
        }

        public TViewModel Model { get; set; }

        public object GetModel()
        {
            return Model;
        }
    }

    public interface IFubuRazorView : IRenderableView, ITemplate
    {
        void RenderPartial();
        ITemplate Layout { get; set; }
        Func<string, string> SiteResource { get; set; }
    }

    public static class FubuRazorViewExtensions
    {
        public static IFubuRazorView Modify(this IFubuRazorView view, Action<IFubuRazorView> modify)
        {
            modify(view);
            return view;
        }
    }
}