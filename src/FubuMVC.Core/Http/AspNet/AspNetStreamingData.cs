using System.IO;
using System.Web;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetStreamingData : IStreamingData
    {
        private readonly HttpContextBase _context;

        public AspNetStreamingData(HttpContextBase context)
        {
            _context = context;
        }

        public Stream Input
        {
            get { return _context.Request.InputStream; }
        }

        public Stream Output
        {
            get { return _context.Response.OutputStream; } 
        }

        public string OutputContentType
        {
            get
            {
                return _context.Response.ContentType;
            }
            set
            {
                _context.Response.ContentType = value;
            }
        }


    }
}