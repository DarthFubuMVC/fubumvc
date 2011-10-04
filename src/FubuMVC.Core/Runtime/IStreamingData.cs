using System;
using System.IO;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public interface IStreamingData
    {
        Stream Input { get; }
        Stream Output { get;}
        string OutputContentType { get; set; }
    }

    // TODO -- this will NOT work in the OWIN world
    public class StreamingData : IStreamingData
    {
        public Stream Input
        {
            get { return HttpContext.Current.Request.InputStream; }
        }

        public Stream Output
        {
            get { return HttpContext.Current.Response.OutputStream; } 
        }

        public string OutputContentType
        {
            get
            {
                return HttpContext.Current.Response.ContentType;
            }
            set
            {
                HttpContext.Current.Response.ContentType = value;
            }
        }


    }

    public static class StreamingDataExtensions
    {
        public static string InputText(this IStreamingData data)
        {
            var reader = new StreamReader(data.Input);
            return reader.ReadToEnd();
        }

        public static bool HasBodyData(this IStreamingData data)
        {
            return data.Input != null && data.Input.CanRead && data.Input.Length > 0;
        }

        public static bool CouldBeJson(this IStreamingData data)
        {
            if (!data.HasBodyData()) return false;

            var reader = new StreamReader(data.Input);
            var firstCharacter = reader.Read();
            data.Input.Position = 0;

            return firstCharacter == '{';
        }
    }
}