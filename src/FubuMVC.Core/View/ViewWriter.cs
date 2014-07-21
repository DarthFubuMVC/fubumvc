using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using HtmlTags.Conventions;

namespace FubuMVC.Core.View
{
    public interface IViewWriter
    {
        IViewToken View { get; }
    }

    public class ViewWriter<T> : IMediaWriter<T>, IViewWriter, DescribesItself where T : class
    {
        private readonly ITemplateFile _view;

        public ViewWriter(ITemplateFile view)
        {
            _view = view;
        }

        public IViewToken View
        {
            get { return _view; }
        }

        public void Write(string mimeType, IFubuRequestContext context, T resource)
        {
            IRenderableView view = BuildView(context);
            view.Page.ServiceLocator = context.Services;
            view.Page.As<IFubuPage<T>>().Model = resource;

            // TODO -- clean this up.
            if (_view.ProfileName.IsNotEmpty())
            {
                view.Page.Get<ActiveProfile>().Push(_view.ProfileName);
            }

            view.Render(context);

            if (_view.ProfileName.IsNotEmpty())
            {
                view.Page.Get<ActiveProfile>().Pop();
            }
        }


        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }

        public IRenderableView BuildView(IFubuRequestContext context)
        {
            if (context.Request.IsAjaxRequest() || context.Services.GetInstance<ICurrentChain>().IsInPartial())
            {
                return _view.GetPartialView();
            }

            return _view.GetView();
        }

        public void Describe(Description description)
        {
            description.Title = Description.For(_view).Title;
            description.ShortDescription = null;
            description.AddChild("View", _view);
        }
    }


}