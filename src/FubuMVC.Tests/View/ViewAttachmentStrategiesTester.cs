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
        [SetUp]
        public void SetUp()
        {
            token = new FakeViewToken
            {
                ViewType = typeof(AAction),
                ViewModelType = typeof (ViewModel1)
            };
            var views = new List<IDiscoveredViewToken>
            {
                token
            };

            bag = new ViewBag(views);

            strategy = new TypeAndNamespaceAndName();
        }

        private FakeViewToken token;
        private ViewBag bag;
        private TypeAndNamespaceAndName strategy;

        [Test]
        public void everything_matches()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            strategy.Find(action, bag).ShouldBeTheSameAs(token);
        }

        [Test]
        public void only_name_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.ViewModelType = typeof (ViewModel2);

            strategy.Find(action, bag).ShouldBeNull();
        }

        [Test]
        public void only_type_and_name_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.ViewType = typeof(SubNamespace.AAction);

            strategy.Find(action, bag).ShouldBeNull();
        }

        [Test]
        public void only_type_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.ViewType = typeof(SomeOtherView);

            strategy.Find(action, bag).ShouldBeNull();
        }
    }

    [TestFixture]
    public class TypeAndNamespaceTester
    {
        [SetUp]
        public void SetUp()
        {
            token = new FakeViewToken
            {
                ViewType = typeof(FakeViewToken),
                ViewModelType = typeof (ViewModel1)
            };
            var views = new List<IDiscoveredViewToken>
            {
                token
            };

            bag = new ViewBag(views);

            strategy = new TypeAndNamespace();
        }

        private FakeViewToken token;
        private ViewBag bag;
        private TypeAndNamespace strategy;

        [Test]
        public void everything_matches()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            strategy.Find(action, bag).ShouldBeTheSameAs(token);
        }

        [Test]
        public void only_name_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.ViewModelType = typeof (ViewModel2);

            strategy.Find(action, bag).ShouldBeNull();
        }

        [Test]
        public void only_type_and_name_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.ViewType = typeof (SubNamespace.AAction);

            strategy.Find(action, bag).ShouldBeNull();
        }

        [Test]
        public void only_type_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            token.ViewType = typeof (SomeOtherView);

            strategy.Find(action, bag).ShouldBeTheSameAs(token);
        }
    }


    public class FakeViewToken : BehaviorNode, IDiscoveredViewToken, IViewToken
    {
        public override BehaviorCategory Category { get { return BehaviorCategory.Output; } }

        public Type ViewModelType { get; set; }

        public Type ViewType { get; set; }

        public IViewToken ToViewToken()
        {
            return this;
        }

        public BehaviorNode ToBehavioralNode()
        {
            throw new NotImplementedException();
        }

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

    namespace SubNamespace
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