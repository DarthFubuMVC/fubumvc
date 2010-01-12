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
                Name = "A",
                Namespace = GetType().Namespace,
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
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.A());
            strategy.Find(action, bag).First().ShouldBeTheSameAs(token);
        }

        [Test]
        public void only_name_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.A());
            token.ViewModelType = typeof (ViewModel2);

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_name_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.A());
            token.Namespace = Guid.NewGuid().ToString();

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.A());
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
                Namespace = GetType().Namespace,
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
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.A());
            strategy.Find(action, bag).First().ShouldBeTheSameAs(token);
        }

        [Test]
        public void only_name_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.A());
            token.ViewModelType = typeof (ViewModel2);

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_name_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.A());
            token.Namespace = Guid.NewGuid().ToString();

            strategy.Find(action, bag).Count().ShouldEqual(0);
        }

        [Test]
        public void only_type_and_namespace_match()
        {
            ActionCall action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.A());
            token.Name = "something different";

            strategy.Find(action, bag).First().ShouldBeTheSameAs(token);
        }
    }


    public class FakeViewToken : BehaviorNode, IViewToken
    {
        public override BehaviorCategory Category { get { return BehaviorCategory.Output; } }

        public Type ViewModelType { get; set; }

        public string Namespace { get; set; }

        public string Name { get; set; }

        public BehaviorNode ToBehavioralNode()
        {
            throw new NotImplementedException();
        }

        protected override ObjectDef buildObjectDef()
        {
            throw new NotImplementedException();
        }
    }

    public class ViewAttachmentStrategiesTesterController
    {
        public ViewModel1 A()
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