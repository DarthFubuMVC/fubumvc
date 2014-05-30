using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class ViewEngineSettingsTester
    {
        private ViewEngineSettings _runner;

        [SetUp]
        public void Setup()
        {
            var types = new TypePool();
            types.AddAssembly(GetType().Assembly);

            _runner = new ViewEngineSettings();
        }

        [Test]
        public void the_default_shared_folders_are_just_Shared()
        {
            _runner.SharedLayoutFolders.Single().ShouldEqual("Shared");
        }

        [Test]
        public void the_default_application_layout_is_Application()
        {
            _runner.ApplicationLayoutName.ShouldEqual("Application");
        }

        [Test]
        public void should_not_add_same_type_of_facility_more_than_once()
        {
            _runner.AddFacility(new TestViewFacility());
            _runner.AddFacility(new TestViewFacility());

            _runner.Facilities.ShouldHaveCount(1);
        }

        [Test]
        public void default_logic_for_is_shared()
        {
            _runner.IsSharedFolder(@"foo/bar/Shared").ShouldBeTrue();
            _runner.IsSharedFolder("Shared").ShouldBeTrue();

            _runner.IsSharedFolder("Shared/Something").ShouldBeFalse();
            _runner.IsSharedFolder("Foo/Bar").ShouldBeFalse();
        }

        [Test]
        public void customize_is_shared_logic()
        {
            _runner.SharedLayoutFolders.Clear();
            _runner.SharedLayoutFolders.Add("Layouts");
            _runner.SharedLayoutFolders.Add("Layouts2");

            _runner.IsSharedFolder("Shared").ShouldBeFalse();
            _runner.IsSharedFolder("Layouts").ShouldBeTrue();
            _runner.IsSharedFolder("Layouts2").ShouldBeTrue();
        }

        [Test]
        public void should_ignore_folders()
        {
            var settings = new ViewEngineSettings();
            settings.FolderShouldBeIgnored("/foo/fubu-content/").ShouldBeTrue();
            settings.FolderShouldBeIgnored("/foo/fubu-content").ShouldBeTrue();
            settings.FolderShouldBeIgnored("fubu-content").ShouldBeTrue();
            settings.FolderShouldBeIgnored("bin").ShouldBeTrue();
            settings.FolderShouldBeIgnored("obj").ShouldBeTrue();
            settings.FolderShouldBeIgnored("node_modules").ShouldBeTrue();
            settings.FolderShouldBeIgnored("node_modules/").ShouldBeTrue();
            settings.FolderShouldBeIgnored("/foo/node_modules/").ShouldBeTrue();
            settings.FolderShouldBeIgnored("/foo/bar/node_modules/").ShouldBeTrue();

            settings.FolderShouldBeIgnored("ok").ShouldBeFalse();
            settings.FolderShouldBeIgnored("ok/").ShouldBeFalse();
            settings.FolderShouldBeIgnored("/ok").ShouldBeFalse();
            settings.FolderShouldBeIgnored("/ok/good/alright").ShouldBeFalse();
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

            public void AttachLayouts(string defaultLayoutName, IViewFacility facility, ITemplateFolder folder)
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

            public void AttachViewModels(Assembly defaultAssembly, ViewTypePool types, ITemplateLogger logger)
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
                return
                    Task.Factory.StartNew(
                        () => { return new ITemplateFile[] {new TestViewToken()} as IEnumerable<ITemplateFile>; });
            }

            public BehaviorNode CreateViewNode(Type type)
            {
                return null;
            }

            public void Fill(ViewEngineSettings settings, BehaviorGraph graph)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IViewToken> AllViews()
            {
                throw new NotImplementedException();
            }

            public ITemplateFile FindInShared(string viewName)
            {
                throw new NotImplementedException();
            }

            public ViewEngineSettings Settings { get; set; }
            public Type TemplateType { get; private set; }
            public Task LayoutAttachment { get; private set; }
            public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
            {
                throw new NotImplementedException();
            }

            public void ReadSharedNamespaces(CommonViewNamespaces namespaces)
            {
                throw new NotImplementedException();
            }
        }
    }
}