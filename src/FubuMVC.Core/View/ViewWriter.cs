using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View
{
    public class ViewWriter<T> : IMediaWriter<T>
    {
        private readonly ICurrentChain _chains;
        private readonly IViewFactory _factory;
        private readonly IRequestHeaders _headers;

        public ViewWriter(ICurrentChain chains, IViewFactory factory, IRequestHeaders headers)
        {
            _chains = chains;
            _factory = factory;
            _headers = headers;
        }

        public void Write(string mimeType, T resource)
        {
            BuildView().Render();
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
    }
}