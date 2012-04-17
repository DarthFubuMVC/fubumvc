using System;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ConnegAttributeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            theGraph = registry.BuildGraph();
        }

        #endregion

        private BehaviorGraph theGraph;

        private BehaviorChain chainFor(Expression<Action<Controller1>> action)
        {
            return theGraph.BehaviorFor(action);
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

        [Test]
        public void default_behavior()
        {
            var chain = chainFor(x => x.All(null));
            chain.Input.AllowHttpFormPosts.ShouldBeTrue();
            chain.Input.UsesFormatter<JsonFormatter>().ShouldBeTrue();
            chain.Input.UsesFormatter<XmlFormatter>().ShouldBeTrue();
        }

        [Test]
        public void json_and_html()
        {
            var chain = chainFor(x => x.JsonAndHtml(null));
            chain.Input.AllowHttpFormPosts.ShouldBeTrue();
            chain.Input.UsesFormatter<JsonFormatter>().ShouldBeTrue();
            chain.Input.UsesFormatter<XmlFormatter>().ShouldBeFalse();
        }

        [Test]
        public void json_only()
        {
            var chain = chainFor(x => x.JsonOnly(null));
            chain.Input.AllowHttpFormPosts.ShouldBeFalse();
            chain.Input.UsesFormatter<JsonFormatter>().ShouldBeTrue();
            chain.Input.UsesFormatter<XmlFormatter>().ShouldBeFalse();
        }

        [Test]
        public void xml_and_html()
        {
            var chain = chainFor(x => x.XmlAndHtml(null));
            chain.Input.AllowHttpFormPosts.ShouldBeTrue();
            chain.Input.UsesFormatter<JsonFormatter>().ShouldBeFalse();
            chain.Input.UsesFormatter<XmlFormatter>().ShouldBeTrue();
        }

        [Test]
        public void xml_and_json_only()
        {
            var chain = chainFor(x => x.XmlAndJson(null));
            chain.Input.AllowHttpFormPosts.ShouldBeFalse();

            chain.Input.UsesFormatter<JsonFormatter>().ShouldBeTrue();
            chain.Input.UsesFormatter<XmlFormatter>().ShouldBeTrue();
        }

        [Test]
        public void xml_only()
        {
            var chain = chainFor(x => x.XmlOnly(null));
            chain.Input.AllowHttpFormPosts.ShouldBeFalse();
            chain.Input.UsesFormatter<JsonFormatter>().ShouldBeFalse();
            chain.Input.UsesFormatter<XmlFormatter>().ShouldBeTrue();
        }
    }
}