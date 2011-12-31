using System;
using System.IO;
using System.Text;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using RazorEngine.Templating;

namespace FubuMVC.Razor.Rendering
{
    public abstract class FubuRazorView : TemplateBase, IFubuRazorView, IMayHaveLayout
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

        public virtual void Run()
        {
            var result = ((ITemplate) this).Run(new ExecuteContext());
            ServiceLocator.GetInstance<IOutputWriter>().WriteHtml(result);
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

        public HtmlTag Tag(string tagName)
        {
            return new HtmlTag(tagName);
        }

        public string H(object value)
        {
            //return Get<IHtmlEncoder>().Encode(value);
            return null;
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

        public override void Run()
        {
            SetModel(ServiceLocator.GetInstance<IFubuRequest>());
            base.Run();
        }

        public TViewModel Model { get; set; }

        public object GetModel()
        {
            return Model;
        }
    }

    public interface IFubuRazorView : IFubuPage
    {
        //Dictionary<string, TextWriter> Content { set; get; }
        //Dictionary<string, string> OnceTable { set; get; }
        //Dictionary<string, object> Globals { set; get; }
        //TextWriter Output { get; set; }

        void Run();
        //Guid GeneratedViewId { get; }

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

    public class FubuRazorViewDecorator : IFubuRazorView
    {
        private readonly IFubuRazorView _view;
        public FubuRazorViewDecorator(IFubuRazorView view)
        {
            _view = view;
            PreRender = new CompositeAction<IFubuRazorView>();
            PostRender = new CompositeAction<IFubuRazorView>();
        }

        public CompositeAction<IFubuRazorView> PreRender { get; set; }
        public CompositeAction<IFubuRazorView> PostRender { get; set; }

        public void Run()
        {
            PreRender.Do(_view);
            _view.Run();
            PostRender.Do(_view);
        }

        public Func<string, string> SiteResource
        {
            get { return _view.SiteResource; }
            set { _view.SiteResource = value; }
        }

        public string ElementPrefix
        {
            get { return _view.ElementPrefix; }
            set { _view.ElementPrefix = value; }
        }

        public IServiceLocator ServiceLocator
        {
            get { return _view.ServiceLocator; }
            set { _view.ServiceLocator = value; }
        }

        public IUrlRegistry Urls
        {
            get { return _view.Urls; }
        }

        public T Get<T>()
        {
            return _view.Get<T>();
        }

        public T GetNew<T>()
        {
            return _view.GetNew<T>();
        }

        public void Write(object content)
        {
            _view.Write(content);
        }
    }

    public interface IMayHaveLayout : ITemplate
    {
        ITemplate Layout { get; set; }
    }
}