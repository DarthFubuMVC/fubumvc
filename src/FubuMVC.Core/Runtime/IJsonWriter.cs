﻿using System;
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
}