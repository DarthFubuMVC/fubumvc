namespace FubuMVC.HelloWorld.Controllers.Location
{
    public class LocationController
    {
        public LocationViewModel WhereAreWe(LocationViewModel whereAreWe)
        {
            return whereAreWe;
        }
    }

    public class LocationViewModel
    {
        public string RawUrl { get; set; }
    }
}