using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class WriteStringTester
    {
        [Test]
        public void to_object_def_builds_a_string_writer()
        {
            new WriteString().As<IContainerModel>()
                .ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IMediaWriter<string>>()
                .Type
                .ShouldEqual(typeof (StringWriter));
        }
    }
}