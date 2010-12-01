using System.Net.Mime;

namespace FubuMVC.Core.Runtime
{
    public class MimeType
    {
        public static readonly MimeType Html = new MimeType(MediaTypeNames.Text.Html);
        public static readonly MimeType Json = new MimeType("application/json");
        public static readonly MimeType Text = new MimeType(MediaTypeNames.Text.Plain);
        public static readonly MimeType Javascript = new MimeType("text/javascript");
        private readonly string _mimeType;

		public MimeType(string mimeType)
        {
            _mimeType = mimeType;
        }

        public override string ToString()
        {
            return _mimeType;
        }
    }
}