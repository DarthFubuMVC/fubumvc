using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class ViewEnginesTester
    {
        private ViewEngines _runner;

        [SetUp]
        public void Setup()
        {
            var types = new TypePool();
            types.AddAssembly(GetType().Assembly);

            _runner = new ViewEngines();
        }

        [Test]
        public void should_not_add_same_type_of_facility_more_than_once()
        {
            _runner.AddFacility(new TestViewFacility());
            _runner.AddFacility(new TestViewFacility());

            _runner.Facilities.ShouldHaveCount(1);
        }


        public class TestViewToken : ITemplateFile
        {
            public IRenderableView GetView()
            {
                throw new NotImplementedException();
            }

            public IRenderableView GetPartialView()
            {
                throw new NotImplementedException();
            }

            public string ProfileName { get; set; }
            public string FilePath { get; private set; }
            public string RootPath { get; private set; }
            public string Origin { get; private set; }
            public string ViewPath { get; private set; }
            public Parsing Parsing { get; private set; }
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

            public bool FromHost()
            {
                throw new NotImplementedException();
            }

            public bool IsPartial()
            {
                throw new NotImplementedException();
            }

            public string FullName()
            {
                throw new NotImplementedException();
            }

            public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
            {
                throw new NotImplementedException();
            }

            public ITemplateFile Master { get; set; }

            public Type ViewModel
            {
                get { throw new NotImplementedException(); }
            }

            public string Name()
            {
                throw new NotImplementedException();
            }

            public string Namespace
            {
                get { throw new NotImplementedException(); }
            }

            public override bool Equals(object obj)
            {
                return obj.GetType() == GetType();
            }

            public bool Equals(TestViewToken other)
            {
                return !ReferenceEquals(null, other);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class TestViewFacility : IViewFacility
        {
            public Task<IEnumerable<ITemplateFile>> FindViews(BehaviorGraph graph)
            {
                return Task.Factory.StartNew(() => {
                    return new ITemplateFile[]{new TestViewToken()} as IEnumerable<ITemplateFile>;
                });

            }

            public BehaviorNode CreateViewNode(Type type) { return null; }
        }
    }
}