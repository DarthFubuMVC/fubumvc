using System.Web;
using RazorEngine.Text;

namespace FubuMVC.Razor.Rendering
{
    public class EncodedString : IEncodedString
    {
        private readonly object _value;

        public EncodedString(object value)
        {
            _value = value;
        }

        public string ToEncodedString()
        {
            return HttpUtility.HtmlEncode(_value);
        }

        public override string ToString()
        {
            return HttpUtility.HtmlEncode(_value);
        }
    }
}