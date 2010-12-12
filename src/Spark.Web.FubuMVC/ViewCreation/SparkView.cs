using System;
using System.Web;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;
using FubuMVC.Core.Urls;
using FubuCore.Util;
using Spark;
using System.IO;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public abstract class SparkView : SparkViewBase, ISparkView, IFubuPage
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();
        private string _siteRoot;

        public SparkView()
        {
            _services.OnMissing = type => { return ServiceLocator.GetInstance(type); };
        }

        public IResourcePathManager ResourcePathManager { get; set; }
        string IFubuPage.ElementPrefix { get; set; }
        public IServiceLocator ServiceLocator { get; set; }
        public ViewContext ViewContext { get; set; }

        public string SiteRoot
        {
            get
            {
                var context = Get<HttpContextBase>();
                if (_siteRoot == null)
                {
                    var appPath = context.Request.ApplicationPath;
                    if (string.IsNullOrEmpty(appPath) || string.Equals(appPath, "/"))
                    {
                        _siteRoot = string.Empty;
                    }
                    else
                    {
                        _siteRoot = "/" + appPath.Trim('/');
                    }
                }
                return _siteRoot;
            }
        }
        public string SiteResource(string path)
        {
            return ResourcePathManager.GetResourcePath(SiteRoot, path);
        }
        public string H(object value)
        {
            return HttpUtility.HtmlEncode(value.ToString());
        }
        public object HTML(object value)
        {
            return value;
        }
        public T Get<T>()
        {
            return (T)_services[typeof(T)];
        }
        public T GetNew<T>()
        {
            return ServiceLocator.GetInstance<T>();
        }
        public IUrlRegistry Urls
        {
            get { return Get<IUrlRegistry>(); }
        }
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            ViewContext = viewContext;
            var outerView = ViewContext.View as SparkView;
            var isNestedView = outerView != null && ReferenceEquals(this, outerView) == false;

            if (isNestedView && outerView.Output != null)
                writer = outerView.Output;

            var priorContent = Content;
            var priorOnce = OnceTable;
            TextWriter priorContentView = null;

            if (isNestedView)
            {
                // set aside the "view" content, to avoid modification
                if (outerView.Content.TryGetValue("view", out priorContentView))
                    outerView.Content.Remove("view");

                // assume the values of the outer view collections
                Content = outerView.Content;
                OnceTable = outerView.OnceTable;
            }

            RenderView(writer);

            if (isNestedView)
            {
                Content = priorContent;
                OnceTable = priorOnce;

                // restore previous state of "view" content
                if (priorContentView != null)
                    outerView.Content["view"] = priorContentView;
                else if (outerView.Content.ContainsKey("view"))
                    outerView.Content.Remove("view");
            }
            else
            {
                // proactively dispose named content. pools spoolwriter pages. avoids finalizers.
                foreach (var content in Content.Values) content.Close();
            }

            Content.Clear();
        }
    }

    public abstract class SparkView<TModel> : SparkView, IFubuPage<TModel> where TModel : class
    {
        public TModel Model { get; private set; }

        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<TModel>();
        }
        public void SetModel(object model)
        {
            throw new NotImplementedException();
        }
    }
}