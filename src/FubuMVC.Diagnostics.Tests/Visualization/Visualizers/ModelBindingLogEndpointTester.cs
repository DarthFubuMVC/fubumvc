using System;
using System.Diagnostics;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.Logging;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.UI.Bootstrap.Collapsibles;
using FubuMVC.Diagnostics.Visualization.Visualizers;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Visualization.Visualizers
{
    [TestFixture]
    public class ModelBindingLogEndpointTester
    {
        private ModelBindingLog theLog;

        [SetUp]
        public void SetUp()
        {
            theLog = new ModelBindingLog(){
                Report = new BindingReport(typeof (FakeModel), new FakeModelBinder())
            };
        }

        [Test]
        public void just_show_the_selected_model_binder_if_no_properties()
        {
            var tag = new ModelBindingFubuDiagnostics(null).VisualizePartial(theLog).As<CollapsibleTag>();

            tag.ToString().ShouldContain("Used Title of FakeModelBinder");
        }

        [Test]
        public void show_with_properties()
        {
            theLog.Report.AddProperty(ReflectionHelper.GetProperty<FakeModel>(x => x.Name), new ConversionPropertyBinder(new BindingRegistry()));
            theLog.Report.Used(new BindingValue{RawKey = "somethingelse", RawValue = "raw", Source = "the request"});

            var tag = new ModelBindingFubuDiagnostics(null).VisualizePartial(theLog).As<CollapsibleTag>();
            tag.ToString().ShouldContain("<tr><td>Name</td><td>ConversionPropertyBinder</td><td>&#39;raw&#39; from &#39;the request&#39;/somethingelse</td></tr>");
        }
    }

    public class FakeModel
    {
        public string Name { get; set; }
    }

    [Title("Title of FakeModelBinder")]
    public class FakeModelBinder : IModelBinder
    {
        public bool Matches(Type type)
        {
            throw new NotImplementedException();
        }

        public object Bind(Type type, IBindingContext context)
        {
            throw new NotImplementedException();
        }
    }
}