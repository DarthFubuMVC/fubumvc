using System;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Media.Formatters;
using NUnit.Framework;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ConnegAttributeTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            theGraph = registry.BuildGraph();
        }

        private BehaviorChain chainFor(Expression<Action<Controller1>> action)
        {
            return theGraph.BehaviorFor(action);
        }

        [Test]
        public void default_behavior()
        {
            var chain = chainFor(x => x.All(null));
            chain.ConnegInputNode().FormatterUsage.ShouldEqual(FormatterUsage.all);
            chain.ConnegInputNode().AllowHttpFormPosts.ShouldBeTrue();
        
            chain.ConnegOutputNode().FormatterUsage.ShouldEqual(FormatterUsage.all);
        }

        [Test]
        public void xml_only()
        {
            var chain = chainFor(x => x.XmlOnly(null));
            chain.ConnegInputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegInputNode().AllowHttpFormPosts.ShouldBeFalse();
            chain.ConnegInputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(XmlFormatter));

            chain.ConnegOutputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegOutputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(XmlFormatter));
        }

        [Test]
        public void json_only()
        {
            var chain = chainFor(x => x.JsonOnly(null));
            chain.ConnegInputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegInputNode().AllowHttpFormPosts.ShouldBeFalse();
            chain.ConnegInputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));

            chain.ConnegOutputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegOutputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));
        }

        [Test]
        public void xml_and_json_only()
        {
            var chain = chainFor(x => x.XmlAndJson(null));
            chain.ConnegInputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegInputNode().AllowHttpFormPosts.ShouldBeFalse();
            chain.ConnegInputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter), typeof(XmlFormatter));

            chain.ConnegOutputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegOutputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter), typeof(XmlFormatter));
        }

        [Test]
        public void xml_and_html()
        {
            var chain = chainFor(x => x.XmlAndHtml(null));
            chain.ConnegInputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegInputNode().AllowHttpFormPosts.ShouldBeTrue();
            chain.ConnegInputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(XmlFormatter));

            chain.ConnegOutputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegOutputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(XmlFormatter));
        }

        [Test]
        public void json_and_html()
        {
            var chain = chainFor(x => x.JsonAndHtml(null));
            chain.ConnegInputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegInputNode().AllowHttpFormPosts.ShouldBeTrue();
            chain.ConnegInputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));

            chain.ConnegOutputNode().FormatterUsage.ShouldEqual(FormatterUsage.selected);
            chain.ConnegOutputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));
        }

        public class ViewModel1
        {
        }

        public class ViewModel2
        {
        }

        public class ViewModel3
        {
        }

        public class ViewModel4
        {
        }

        public class ViewModel5
        {
        }


        public class Controller1
        {
            [Conneg]
            public ViewModel1 All(ViewModel1 m)
            {
                return null;
            }

            [Conneg(FormatterOptions.Xml)]
            public ViewModel2 XmlOnly(ViewModel1 m)
            {
                return null;
            }

            [Conneg(FormatterOptions.Json)]
            public ViewModel3 JsonOnly(ViewModel1 m)
            {
                return null;
            }

            [Conneg(FormatterOptions.Xml | FormatterOptions.Json)]
            public ViewModel3 XmlAndJson(ViewModel1 m)
            {
                return null;
            }

            [Conneg(FormatterOptions.Xml | FormatterOptions.Html)]
            public ViewModel3 XmlAndHtml(ViewModel1 m)
            {
                return null;
            }

            [Conneg(FormatterOptions.Json | FormatterOptions.Html)]
            public ViewModel3 JsonAndHtml(ViewModel1 m)
            {
                return null;
            }
        }
    }
}