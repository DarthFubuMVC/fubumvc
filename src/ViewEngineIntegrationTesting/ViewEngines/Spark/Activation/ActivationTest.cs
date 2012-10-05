using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuMVC.Core;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace ViewEngineIntegrationTesting.ViewEngines.Spark.Activation
{
    public class spark_views_are_activated_with_services_and_the_model_is_attached : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ActivationEndpoint>();
            registry.Services(x => {
                x.SetServiceIfNone<IActivationRenderer, ActivationRenderer>();
            });
        }

        [Test]
        public void the_services_and_model_are_attached_while_running_the_view()
        {
            endpoints.Get<ActivationEndpoint>(x => x.get_model()).ReadAsText()
                .ShouldContain("the model is named Jeremy");

        }
    }

    public class ActivationEndpoint
    {
        public ActivationModel get_model()
        {
            return new ActivationModel {Name = "Jeremy"};
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
