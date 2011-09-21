namespace FubuMVC.HelloSpark.Controllers.Earth
{
    public class EarthController
    {
        public EarthViewModel<string> Rock(EarthViewModel<string> whereAreWe)
        {
            return whereAreWe;
        }
    }

    public class EarthViewModel<T>
    {
        public T RawUrl { get; set; }
    }
}