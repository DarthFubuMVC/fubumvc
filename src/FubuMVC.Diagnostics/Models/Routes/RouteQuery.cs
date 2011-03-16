using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Models.Routes
{
    public class RouteQuery
    {
        [QueryString]
        public int page { get; set; }
        [QueryString]
        public int rows { get; set; }
        [QueryString]
        public string sidx { get; set; }
        [QueryString]
        public string sord { get; set; }
    }
}