using FubuCore;
using FubuCore.Binding;
using HtmlTags;

namespace FubuMVC.Core.Runtime
{
    public interface IJsonWriter
    {
        void Write(object output);
        void Write(object output, string mimeType);
    }

    public class JsonWriter : IJsonWriter
    {
        private readonly IOutputWriter _outputWriter;

        public JsonWriter(IOutputWriter outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public void Write(object output)
        {
            Write(output, MimeType.Json.ToString());
        }

        public void Write(object output, string mimeType)
        {
            _outputWriter.Write(mimeType, JsonUtil.ToJson(output));
        }
    }


    public class AjaxAwareJsonWriter : IJsonWriter
    {
        private readonly IOutputWriter _outputWriter;
        private readonly IRequestData _requestData;

        public AjaxAwareJsonWriter(IOutputWriter outputWriter, IRequestData requestData)
        {
            _outputWriter = outputWriter;
            _requestData = requestData;
        }

        public void Write(object output)
        {
            string rawJsonOutput = JsonUtil.ToJson(output);
            if (_requestData.IsAjaxRequest())
            {
                _outputWriter.Write(MimeType.Json.ToString(), rawJsonOutput);
            }
            else
            {
                // For proper jquery.form plugin support of file uploads
                // See the discussion on the File Uploads sample at http://malsup.com/jquery/form/#code-samples
                string html = "<html><body><textarea rows=\"10\" cols=\"80\">" + rawJsonOutput +
                    "</textarea></body></html>";
                _outputWriter.Write(MimeType.Html.ToString(), html);
            }
        }

        // TODO -- pull this out into Conneg as a different media writer
        public void Write(object output, string mimeType)
        {
            Write(output);
        }
    }

    public class JsonpAwareWriter : IJsonWriter
    {
        private readonly IOutputWriter _outputWriter;
        private readonly IRequestData _requestData;

        public const string JsonPHttpRequest = "jsonp";

        public JsonpAwareWriter(IOutputWriter outputWriter, IRequestData requestData)
        {
            _outputWriter = outputWriter;
            _requestData = requestData;
        }

        public void Write(object output)
        {
            Write(output, MimeType.Json.ToString());
        }

        public void Write(object output, string mimeType)
        {
            var json = JsonUtil.ToJson(output);
            var padding = GetJsonPadding(_requestData);

            if (padding != null)
            {
                json = "{0}({1});".ToFormat(padding, json);
            }

            _outputWriter.Write(mimeType, json);
        }

        public static string GetJsonPadding(IRequestData requestInput)
        {
            string result = null;
            requestInput.Value(JsonPHttpRequest, value => result = value.ToString());
            return result;
        }
    }

}