using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;
using FubuMVC.Tests.View.FakeViews;
using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class TypeAndNamespaceAndNameTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            token = new FakeViewToken
            {
                Name = "AAction",
                Folder = GetType().Namespace,
                ViewType =  typeof(AAction),
                ViewModelType = typeof (ViewModel1)
            };
            var views = new List<IViewToken>
            {
                token
            };

            bag = new ViewBag(views);

            strategy = new TypeAndNamespaceAndName();
        }

        #endregion

        private FakeViewToken token;
        private ViewBag bag;
        private TypeAndNamespaceAndName strategy;

        [Test]
        public void everything_matches()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            strategy.Find(action, bag).First().ShouldBeTheSameAs(token);
        }

        [Test]
        public void only_name_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.ViewModelType = typeof (ViewModel2);

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_name_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.Folder = Guid.NewGuid().ToString();

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.Name = "something different";

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }
    }

    [TestFixture]
    public class TypeAndNamespaceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            token = new FakeViewToken
            {
                Name = "A",
                Folder = GetType().Namespace,
                ViewType = typeof(FakeViewToken),
                ViewModelType = typeof (ViewModel1)
            };
            var views = new List<IViewToken>
            {
                token
            };

            bag = new ViewBag(views);

            strategy = new TypeAndNamespace();
        }

        #endregion

        private FakeViewToken token;
        private ViewBag bag;
        private TypeAndNamespace strategy;

        [Test]
        public void everything_matches()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            strategy.Find(action, bag).First().ShouldBeTheSameAs(token);
        }

        [Test]
        public void only_name_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.ViewModelType = typeof (ViewModel2);

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_name_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.Folder = Guid.NewGuid().ToString();

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.Name = "something different";

            strategy.Find(action, bag).First().ShouldBeTheSameAs(token);
        }
    }


    public class FakeViewToken : BehaviorNode, IViewToken
    {
        public override BehaviorCategory Category { get { return BehaviorCategory.Output; } }

        public Type ViewModelType { get; set; }

        public string Folder { get; set; }

        public string Name { get; set; }

        public BehaviorNode ToBehavioralNode()
        {
            throw new NotImplementedException();
        }

        public Type ViewType { get; set;}

        protected override ObjectDef buildObjectDef()
        {
            throw new NotImplementedException();
        }
    }

    public class AAction
    {

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

    public class ViewAttachmentStrategiesTesterController
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