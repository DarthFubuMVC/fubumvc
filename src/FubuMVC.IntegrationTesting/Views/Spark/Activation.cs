using System.Collections.Generic;
using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.View;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class Activation:ViewIntegrationContext
    {
        public Activation()
        {
            SparkView<ActivationModel>("ActivationView").Write(@"

<div>!{this.Get<IActivationRenderer>().Print(Model)}</div>
");
        }

        public class MyRegistry : FubuRegistry
        {
            public MyRegistry()
            {
                Actions.IncludeType<ActivationEndpoint>();
                Services(x =>
                {
                    x.SetServiceIfNone<IActivationRenderer, ActivationRenderer>();
                });

                AlterSettings<CommonViewNamespaces>(x => x.AddForType<ActivationEndpoint>());
            }
        }


        [Test]
        public void the_services_and_model_are_attached_while_running_the_view()
        {
            Views.Views.Each(x => Debug.WriteLine(x.ViewModel));

            Scenario.Get.Action<ActivationEndpoint>(x => x.get_model());
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