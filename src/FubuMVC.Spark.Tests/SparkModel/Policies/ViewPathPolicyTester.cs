using System;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Policies
{
    public class StubTemplate : ITemplate
    {
        public string Origin
        {
            get; set;
        }

        public string FilePath
        {
            get;
            set;
        }

        public string RootPath
        {
            get;
            set;
        }

        public string ViewPath
        {
            get;
            set;
        }

        public ITemplateDescriptor Descriptor
        {
            get;
            set;
        }
    }

    [TestFixture]
    public class ViewPathPolicyTester : InteractionContext<ViewPathPolicy<ITemplate>>
    {
        private StubTemplate _template;
        protected override void beforeEach()
        {
            _template = new StubTemplate{
                Origin = TemplateConstants.HostOrigin,
                RootPath = "root",
                FilePath = FileSystem.Combine("root", "view.spark")
            };
        }

        [Test]
        public void when_origin_is_host_viewpath_is_not_prefixed()
        {
            ClassUnderTest.Apply(_template);
            _template.ViewPath.ShouldEqual("view.spark");
        }

        [Test]
        public void when_origin_is_not_host_view_path_is_prefixed()
        {
            _template.Origin = "Foo";
            ClassUnderTest.Apply(_template);
            _template.ViewPath.ShouldEqual(FileSystem.Combine("_" + _template.Origin, "view.spark"));
        }

		[Test]
        public void it_matches_when_viewpath_is_empty()
		{
		    _template.ViewPath = null;
            ClassUnderTest.Matches(_template).ShouldBeTrue();
        }

        [Test]
        public void it_does_not_matches_when_viewpath_is_not_empty()
        {
            _template.ViewPath = "someone/else/did/this";
            ClassUnderTest.Matches(_template).ShouldBeFalse();
        }
    }
}