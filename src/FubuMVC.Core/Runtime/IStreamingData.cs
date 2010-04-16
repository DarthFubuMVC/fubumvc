using System;
using System.IO;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public interface IStreamingData
    {
        Stream Input { get; }
        Stream Output { get;}
    }

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
    }

    public static class StreamingDataExtensions
    {
        public static string InputText(this IStreamingData data)
        {
            var reader = new StreamReader(data.Input);
            return reader.ReadToEnd();
        }
    }
}