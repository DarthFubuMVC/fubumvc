namespace FubuMVC.HelloSpark.Controllers
{
    public class WaterController
    {
        public WaterViewModel Spray(WaterInputModel model)
        {
            return model.GotAHose()
                       ? new WaterViewModel { Do = "Rain on your parade!" }
                       : new WaterViewModel { Do = "Take a wild guess!" };
        }
    }

    public class WaterInputModel
    {
        public bool GotAHose()
        {
            return true;
        }
    }

    public class WaterViewModel
    {
        public string Do { get; set; }
    }
}