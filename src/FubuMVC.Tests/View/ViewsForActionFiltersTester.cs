using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Tests.View.FakeViews;
using FubuMVC.Tests.View.SubNamesapce;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class when_filtering_views_by_viewmodel_type_and_namespace_and_name
    {
        [SetUp]
        public void SetUp()
        {
            token = new FakeViewToken
            {
                ViewName = "AAction",
                Namespace = GetType().Namespace,
                ViewModel = typeof (ViewModel1)
            };
            var views = new List<IViewToken>
            {
                token
            };

            bag = new ViewBag(views);

            filter = new ActionWithSameNameAndFolderAsViewReturnsViewModelType();
        }

        private FakeViewToken token;
        private ViewBag bag;
        private ActionWithSameNameAndFolderAsViewReturnsViewModelType filter;

        [Test]
        public void everything_matches()
        {
            ActionCall action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            filter.Apply(action, bag).First().ShouldBeTheSameAs(token);
        }

        [Test]
        public void only_name_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            token.ViewModel = typeof (ViewModel2);

            filter.Apply(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_name_match()
        {
            ActionCall action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            token.Namespace = Guid.NewGuid().ToString();

            filter.Apply(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            token.ViewName = "something different";

            filter.Apply(action, bag).Count().ShouldEqual(0);
        }
    }

    [TestFixture]
    public class when_filtering_views_by_viewmodel_type_and_namespace
    {
        [SetUp]
        public void SetUp()
        {
            token = new FakeViewToken
            {
                ViewName = "A",
                Namespace = GetType().Namespace,
                ViewModel = typeof (ViewModel1)
            };
            var views = new List<IViewToken>
            {
                token
            };

            bag = new ViewBag(views);

            filter = new ActionInSameFolderAsViewReturnsViewModelType();
        }

        private FakeViewToken token;
        private ViewBag bag;
        private ActionInSameFolderAsViewReturnsViewModelType filter;

        [Test]
        public void everything_matches()
        {
            ActionCall action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            filter.Apply(action, bag).First().ShouldBeTheSameAs(token);
        }

        [Test]
        public void only_name_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            token.ViewModel = typeof (ViewModel2);

            filter.Apply(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_name_match()
        {
            ActionCall action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            token.Namespace = Guid.NewGuid().ToString();

            filter.Apply(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            token.ViewName = "something different";

            filter.Apply(action, bag).First().ShouldBeTheSameAs(token);
        }
    }


    public class FakeViewToken : BehaviorNode, ITemplateFile
    {
        public override BehaviorCategory Category { get { return BehaviorCategory.Output; } }

        public Type ViewModel { get; set; }

        public string Namespace { get; set; }

        public string ViewName { get; set; }

        public string Name()
        {
            return ViewName;
        }

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
        }

        public ITemplateFile Master { get; set; }

        protected override ObjectDef buildObjectDef()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name() ?? string.Empty;
        }
    }

    public class SomeOtherView
    {
        
    }

    namespace SubNamesapce
    {
        public class AAction
        {
            
        }
    }

    public class ViewsForActionFilterTesterController
    {
        public ViewModel1 AAction()
        {
            return null;
        }

        public ViewModel1 B()
        {
            return null;
        }

        public ViewModel1 C()
        {
            return null;
        }

        public ViewModel1 D()
        {
            return null;
        }

        public ViewModel1 E()
        {
            return null;
        }
    }
}