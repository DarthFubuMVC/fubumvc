using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Rendering;
using HtmlTags.Conventions;

namespace FubuMVC.Core.View
{
    public class ViewWriter<T> : IMediaWriter<T>, DescribesItself where T : class
    {
        private readonly IViewToken _view;

        public ViewWriter(IViewToken view)
        {
            _view = view;
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

            view.Render();

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
        }
    }


}