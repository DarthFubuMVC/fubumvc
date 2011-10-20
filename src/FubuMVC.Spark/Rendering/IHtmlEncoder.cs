using System.Web;

namespace FubuMVC.Spark.Rendering
{
    public interface IHtmlEncoder
    {
        string Encode(object value);
    }

    public class DefaultHtmlEncoder : IHtmlEncoder
    {
        public string Encode(object value)
        {
            return HttpUtility.HtmlEncode(value);
        }
    }
}