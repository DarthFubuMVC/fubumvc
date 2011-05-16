namespace QuickStart.Spark
{
        public class AirController
        {
            public BreatheViewModel Breathe_TakeABreath(AirInputModel model)
            {
                var result = model.TakeABreath.ToLower() == "in"
                    ? new BreatheViewModel { Text = "Breathe in!" }
                    : new BreatheViewModel { Text = "Exhale!" };

                return result;
            }
        }

        public class AirInputModel
        {
            public string TakeABreath { get; set; }
        }

        public class AirViewModel
        {
            public string Text { get; set; }
        }

        public class BreatheViewModel : AirViewModel
        {

        }


}