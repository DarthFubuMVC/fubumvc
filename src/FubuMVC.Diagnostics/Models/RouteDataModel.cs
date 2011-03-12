namespace FubuMVC.Diagnostics.Models
{
    public class RouteDataModel
    {
        public string Id { get; set; }
        public string Route { get; set; }
        public string Action { get; set; }
        public string InputModel { get; set; }
        public string OutputModel { get; set; }
        public string Constraints { get; set; }
    }
}