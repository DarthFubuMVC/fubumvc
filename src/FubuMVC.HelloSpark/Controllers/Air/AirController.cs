namespace FubuMVC.HelloSpark.Controllers.Air
{
    public class AirController
    {
        public AirViewModel TakeABreath()
        {
            return new AirViewModel { Text = "Take a breath?" };
        }

        public BreatheViewModel Breathe(AirInputModel model)
        {
            var result = model.TakeABreath
                ? new BreatheViewModel { Text = "Breathe in!" }
                : new BreatheViewModel { Text = "Exhale!" };

            return result;
        }
    }

    public class AirInputModel
    {
        public bool TakeABreath { get; set; }
    }

    public class AirViewModel
    {
        public string Text { get; set; }
    }

    public class BreatheViewModel : AirViewModel
    {

    }

}