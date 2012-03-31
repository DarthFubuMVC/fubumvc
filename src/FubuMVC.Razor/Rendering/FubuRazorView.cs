using System;
using System.Web;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.RazorModel;
using HtmlTags;
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
            RenderPartialWith = name => base.Include(name);
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

        public Func<string, TemplateWriter> RenderPartialWith { get; set; }

        public void UseLayout(IFubuRazorView layout)
        {
            Layout = layout;
            if (_Layout == null)
            {
                _Layout = Layout.GetType().FullName;
            }
        }

        public void NoLayout()
        {
            Layout = null;
            _Layout = null;
        }

        public IRazorTemplate OriginTemplate { get; set; }

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

        public override TemplateWriter Include(string name)
        {
            return RenderPartialWith(name);
        }

        public ITemplate Layout { get; set; }
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
        ITemplate Layout { get; }
        void UseLayout(IFubuRazorView layout);
        void NoLayout();
        IRazorTemplate OriginTemplate { get; set; }
        Func<string, TemplateWriter> RenderPartialWith { get; set; }
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