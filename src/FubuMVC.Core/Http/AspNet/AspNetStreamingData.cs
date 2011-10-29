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
    }
}