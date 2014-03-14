using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class Razor_view_activation_tester : ViewIntegrationContext
    {
        public Razor_view_activation_tester()
        {
            RazorView<ActivationModel>("Activation").Write(@"
<div>
    @( this.Get<FubuMVC.IntegrationTesting.Views.Razor.IActivationRenderer>().Print(Model) )
</div>

");
        }

        public class ActivationRegistry : FubuRegistry
        {
            public ActivationRegistry()
            {
                Services(x => x.SetServiceIfNone<IActivationRenderer, ActivationRenderer>());
            }
        }

        [Test]
        public void has_the_services_attached_to_the_view()
        {
            Scenario.Get.Action<ActivationEndpoint>(e => e.get_model());
            Scenario.ContentShouldContain("the model is named Jeremy");
        }
    }

    public class ActivationEndpoint
    {
        public ActivationModel get_model()
        {
            return new ActivationModel { Name = "Jeremy" };
        }
    }

    public class ActivationModel
    {
        public string Name { get; set; }
    }

    public interface IActivationRenderer
    {
        string Print(ActivationModel model);
    }

    public class ActivationRenderer : IActivationRenderer
    {
        public string Print(ActivationModel model)
        {
            return "the model is named " + model.Name;
        }
    }
}