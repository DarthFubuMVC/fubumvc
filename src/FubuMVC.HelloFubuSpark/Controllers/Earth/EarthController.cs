namespace FubuMVC.HelloFubuSpark.Controllers.Earth
{
    public class EarthController
    {
        public EarthViewModel Rock(EarthViewModel whereAreWe)
        {
            return whereAreWe;
        }
    }

    public class EarthViewModel
    {
        public string RawUrl { get; set; }
    }
}