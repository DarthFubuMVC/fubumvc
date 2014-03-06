using System;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Policies
{
    public class StubTemplate : ISparkTemplate
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

        public string RelativePath()
        {
            throw new NotImplementedException();
        }

        public string DirectoryPath()
        {
            throw new NotImplementedException();
        }

        public string RelativeDirectoryPath()
        {
            throw new NotImplementedException();
        }

        public string Name()
        {
            throw new NotImplementedException();
        }

        public bool FromHost()
        {
            throw new NotImplementedException();
        }

        public bool IsPartial()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class ViewPathPolicyTester : InteractionContext<ViewPathPolicy<ISparkTemplate>>
    {
        private StubTemplate _template;
        protected override void beforeEach()
        {
            _template = new StubTemplate{
                Origin = ContentFolder.Application,
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