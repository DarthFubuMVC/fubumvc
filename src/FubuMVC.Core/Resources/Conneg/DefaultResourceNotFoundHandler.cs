using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;

namespace FubuMVC.Core.Resources.Conneg
{
    public class DefaultResourceNotFoundHandler : IResourceNotFoundHandler
    {
        private readonly IOutputWriter _writer;

        public DefaultResourceNotFoundHandler(IOutputWriter writer)
        {
            _writer = writer;
        }

        public void HandleResourceNotFound<T>()
        {
            _writer.Write(MimeType.Html, "Resource Not Found");
        }
    }


}