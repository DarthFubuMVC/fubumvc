using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;
using HtmlTags;

namespace FubuMVC.Core.Behaviors
{
    public class RenderJsonBehavior<T> : BasicBehavior where T : class
    {
        private readonly IFubuRequest _request;
        private readonly IRequestData _requestData;
        private readonly IOutputWriter _writer;

        public RenderJsonBehavior(IOutputWriter writer, IFubuRequest request, IRequestData requestData)
            : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
            _requestData = requestData;
        }

        protected override DoNext performInvoke()
        {
            var output = _request.Get<T>();
            string rawJsonOutput = JsonUtil.ToJson(output);

            if (_requestData.IsAjaxRequest())
            {
                _writer.Write(MimeType.Json.ToString(), rawJsonOutput);
            }
            else
            {
                // For proper jquery.form plugin support of file uploads
                // See the discussion on the File Uploads sample at http://malsup.com/jquery/form/#code-samples
                string html = "<html><body><textarea rows=\"10\" cols=\"80\">" + rawJsonOutput +
                              "</textarea></body></html>";
                _writer.Write(MimeType.Html.ToString(), html);
            }

            return DoNext.Continue;
        }
    }
}