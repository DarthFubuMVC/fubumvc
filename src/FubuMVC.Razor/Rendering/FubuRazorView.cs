using System;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using RazorEngine.Templating;

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

        public virtual void Render()
        {
            var result = ((ITemplate) this).Run(new ExecuteContext());
            ServiceLocator.GetInstance<IOutputWriter>().WriteHtml(result);
        }

        public virtual void RenderPartial()
        {
            _Layout = null;
            Render();
        }

        public HtmlTag Tag(string tagName)
        {
            return new HtmlTag(tagName);
        }

        public string H(object value)
        {
            return Get<IHtmlEncoder>().Encode(value);
        }

        public ITemplate Layout { get; set; }
    }

    public abstract class FubuRazorView<TViewModel> : FubuRazorView, IFubuPage<TViewModel> where TViewModel : class
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

        public override void Render()
        {
            SetModel(ServiceLocator.GetInstance<IFubuRequest>());
            base.Render();
        }

        public override void RenderPartial()
        {
            SetModel(ServiceLocator.GetInstance<IFubuRequest>());
            base.RenderPartial();
        }

        public TViewModel Model { get; set; }

        public object GetModel()
        {
            return Model;
        }
    }

    public interface IFubuRazorView : IFubuPage
    {
        void Render();
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