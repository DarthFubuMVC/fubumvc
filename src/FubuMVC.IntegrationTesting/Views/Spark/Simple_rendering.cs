using FubuCore;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class Simple_rendering : ViewIntegrationContext
    {
        public Simple_rendering()
        {
            SparkView<BreatheViewModel>("Breathe")
                .Write(@"
<p>This is real output</p>
<h2>${Model.Text}</h2>");
        }

        [Test]
        public void can_render()
        {
            Scenario.Get.Input(new AirInputModel{TakeABreath = true});
            Scenario.ContentShouldContain("<h2>Breathe in!</h2>");
        }
    }

    public class AirEndpoint
    {
        public AirViewModel TakeABreath(AirRequest request)
        {
            return new AirViewModel { Text = "Take a {0} breath?".ToFormat(request.Type) };
        }

        public BreatheViewModel get_breathe_TakeABreath(AirInputModel model)
        {
            var result = model.TakeABreath
                ? new BreatheViewModel { Text = "Breathe in!" }
                : new BreatheViewModel { Text = "Exhale!" };

            return result;
        }
    }

    public class AirRequest
    {
        public AirRequest()
        {
            Type = "deep";
        }

        public string Type { get; set; }
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