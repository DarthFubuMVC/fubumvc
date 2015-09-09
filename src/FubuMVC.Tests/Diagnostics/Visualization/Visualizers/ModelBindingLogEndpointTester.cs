using System;
using FubuCore.Binding;
using FubuCore.Binding.Logging;
using FubuCore.Descriptions;
using FubuMVC.Core.Diagnostics.Runtime;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics.Visualization.Visualizers
{
    [TestFixture]
    public class ModelBindingLogEndpointTester
    {
        private ModelBindingLog theLog;

        [SetUp]
        public void SetUp()
        {
            theLog = new ModelBindingLog
            {
                Report = new BindingReport(typeof (FakeModel), new FakeModelBinder())
            };
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