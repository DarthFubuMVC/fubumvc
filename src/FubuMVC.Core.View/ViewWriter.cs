using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View
{
    public class ViewWriter<T> : IMediaWriter<T>, DescribesItself
    {
        private readonly ICurrentChain _chains;
        private readonly IViewFactory _factory;
        private readonly IRequestHeaders _headers;
        private readonly IFubuPageActivator _activator;

        public ViewWriter(ICurrentChain chains, IViewFactory factory, IRequestHeaders headers, IFubuPageActivator activator)
        {
            _chains = chains;
            _factory = factory;
            _headers = headers;
            _activator = activator;
        }

        public void Write(string mimeType, T resource)
        {
            IRenderableView view = BuildView();
            _activator.Activate(view.Page);

            view.Render();

            _activator.Deactivate(view.Page);
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }

        public IRenderableView BuildView()
        {
            if (_headers.IsAjaxRequest() || _chains.IsInPartial())
            {
                return _factory.GetPartialView();
            }

            return _factory.GetView();
        }

        public void Describe(Description description)
        {
            description.Title = Description.For(_factory).Title;
        }
    }


}