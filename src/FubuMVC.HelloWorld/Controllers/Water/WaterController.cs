namespace FubuMVC.HelloWorld.Controllers.Water
{
    public class WaterController
    {
        public WaterViewModel<string> Spray(WaterInputModel model)
        {
            return model.GotAHose()
                       ? new WaterViewModel<string> { Do = "Rain on your parade!" }
                       : new WaterViewModel<string> { Do = "Take a wild guess!" };
        }
    }

    public class WaterInputModel
    {
        public bool GotAHose()
        {
            return true;
        }
    }

    //Note: This view model is generic simply to show you can use generics in spark views
    public class WaterViewModel<T>
    {
        public T Do { get; set; }
    }
}