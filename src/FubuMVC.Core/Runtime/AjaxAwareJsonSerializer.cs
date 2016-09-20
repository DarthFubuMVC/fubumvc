﻿using System.ComponentModel;
using System.Threading.Tasks;
using FubuMVC.Core.Json;

namespace FubuMVC.Core.Runtime
{
    [Description("Ajax aware Json serialization with the built in JavaScriptSerializer")]
    public class AjaxAwareJsonSerializer : NewtonsoftJsonFormatter
    {
        public override Task Write<T>(IFubuRequestContext context, T resource, string mimeType)
        {
            var rawJsonOutput = serializeData(context, resource);
            if (context.Request.IsAjaxRequest())
            {
                return context.Writer.Write(MimeType.Json.ToString(), rawJsonOutput);
            }
            else
            {
                // For proper jquery.form plugin support of file uploads
                // See the discussion on the File Uploads sample at http://malsup.com/jquery/form/#code-samples
                var html = "<html><body><textarea rows=\"10\" cols=\"80\">" + rawJsonOutput +
                           "</textarea></body></html>";
                return context.Writer.Write(MimeType.Html.ToString(), html);
            }
        }
    }
}