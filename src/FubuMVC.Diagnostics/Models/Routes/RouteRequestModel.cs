using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Models.Routes
{
    public class RouteRequestModel
    {
		[QueryString]
		public string Column { get; set; }
		[QueryString]
		public string Value { get; set; }
    }
}