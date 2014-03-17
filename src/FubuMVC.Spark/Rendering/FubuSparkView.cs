using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;

using Spark;

namespace FubuMVC.Spark.Rendering
{
    public abstract class FubuSparkView : SparkViewBase, IFubuSparkView
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();

        protected FubuSparkView()
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

        public void Write(object content)
        {
            Output.Write(content);
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
            return Get<IHtmlEncoder>().Encode(value);
        }

        public HtmlString HTML(object value)
        {
            return new HtmlString(value != null ? value.ToString() : null);
        }

        public IFubuPage Page { 
            get { return this; }
        }


        public void Render(IFubuRequestContext context)
        {
            SiteResource = relative => {
                return context.Request.ToFullUrl(relative);
            };

            CacheService = context.Services.GetInstance<ICacheService>();

            context.Writer.Write(MimeType.Html.Value, stream => {
                var writer = new StreamWriter(stream);

                Output = writer;

                RenderView(writer);

                writer.Flush();
            });
        }
    }

    public abstract class FubuSparkView<TViewModel> : FubuSparkView, IFubuPage<TViewModel> where TViewModel : class
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

        public TViewModel Model { get; set; }

        public object GetModel()
        {
            return Model;
        }
    }
}