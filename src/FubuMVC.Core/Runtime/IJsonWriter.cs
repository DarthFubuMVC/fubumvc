using System.Web.Script.Serialization;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Runtime
{
    public interface IJsonSerializer
    {
        T Read<T>(IFubuRequestContext context);
        void Write<T>(T resource, string mimeType, IFubuRequestContext context);
    }

    public class JsonSerializer : IJsonSerializer
    {
        public T Read<T>(IFubuRequestContext context)
        {
            return new JavaScriptSerializer().Deserialize<T>(context.Request.InputText());
        }

        public virtual void Write<T>(T resource, string mimeType, IFubuRequestContext context)
        {
            var text = new JavaScriptSerializer().Serialize(resource);
            context.Writer.Write(mimeType, text);
        }
    }

    public class DataContractJsonSerializer : IJsonSerializer
    {
        public T Read<T>(IFubuRequestContext context)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof (T));
            return (T) serializer.ReadObject(context.Request.Input);
        }

        public void Write<T>(T resource, string mimeType, IFubuRequestContext context)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof (T));

            context.Writer.Write(mimeType, stream => { serializer.WriteObject(stream, resource); });
        }
    }


    public class AjaxAwareJsonSerializer : JsonSerializer
    {
        public override void Write<T>(T resource, string mimeType, IFubuRequestContext context)
        {
            var rawJsonOutput = new JavaScriptSerializer().Serialize(resource);
            if (context.Request.IsAjaxRequest())
            {
                context.Writer.Write(MimeType.Json.ToString(), rawJsonOutput);
            }
            else
            {
                // For proper jquery.form plugin support of file uploads
                // See the discussion on the File Uploads sample at http://malsup.com/jquery/form/#code-samples
                var html = "<html><body><textarea rows=\"10\" cols=\"80\">" + rawJsonOutput +
                           "</textarea></body></html>";
                context.Writer.Write(MimeType.Html.ToString(), html);
            }
        }
    }
}