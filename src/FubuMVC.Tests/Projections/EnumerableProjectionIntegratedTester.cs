using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Resources.Hypermedia;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class EnumerableProjectionIntegratedTester
    {
        private Parent theParent;
        private IProjectionRunner runner;

        [SetUp]
        public void SetUp()
        {
            theParent = new Parent{
                Children = new Child[]{
                    new Child{Name = "Jeremy"},
                    new Child{Name = "Jessica"},
                    new Child{Name = "Natalie"}
                }
            };

            var container = new Container();
            var registry = new FubuRegistry();
            FubuApplication.For(registry).StructureMap(container).Bootstrap();

            runner = container.GetInstance<IProjectionRunner>();
        }

        [Test]
        public void accessors()
        {
            var projection = EnumerableProjection<Parent, Child>.For(x => x.Children);
            projection.As<IProjection<Parent>>().Accessors().Single().ShouldEqual(ReflectionHelper.GetAccessor<Parent>(x => x.Children));
        }

        public XmlElement write(IProjection<Parent> projection)
        {
            var node = XmlNodeCentricMediaNode.ForRoot("root");
            runner.Run(projection, new SimpleValues<Parent>(theParent), node);

            return node.Element;
        }

        [Test]
        public void write_with_inline_projection()
        {
            var projection = new Projection<Parent>(DisplayFormatting.RawValues);
            projection.Enumerable(x => x.Children).DefineProjection(p =>
            {
                p.Value(x => x.Name).Name("name");
            });

            var element = write(projection);

            element.OuterXml.ShouldEqual("<root><Children><Child><name>Jeremy</name></Child><Child><name>Jessica</name></Child><Child><name>Natalie</name></Child></Children></root>");
        }

        [Test]
        public void write_with_precanned_child_projection_with_defaults()
        {
            var projection = new Projection<Parent>(DisplayFormatting.RawValues);
            projection.Enumerable(x => x.Children).UseProjection<SimpleChildProjection>();

            var element = write(projection);

            element.OuterXml.ShouldEqual("<root><Children><Child><name>Jeremy</name></Child><Child><name>Jessica</name></Child><Child><name>Natalie</name></Child></Children></root>");
        }

        [Test]
        public void write_with_precanned_child_projection_overwrite_node()
        {
            var projection = new Projection<Parent>(DisplayFormatting.RawValues);
            projection.Enumerable(x => x.Children).NodeName("children").UseProjection<SimpleChildProjection>();

            var element = write(projection);

            element.OuterXml.ShouldEqual("<root><children><Child><name>Jeremy</name></Child><Child><name>Jessica</name></Child><Child><name>Natalie</name></Child></children></root>");
        }

        [Test]
        public void write_with_precanned_child_projection_overwrite_leaf_name()
        {
            var projection = new Projection<Parent>(DisplayFormatting.RawValues);
            projection.Enumerable(x => x.Children).LeafName("child").UseProjection<SimpleChildProjection>();

            var element = write(projection);

            element.OuterXml.ShouldEqual("<root><Children><child><name>Jeremy</name></child><child><name>Jessica</name></child><child><name>Natalie</name></child></Children></root>");
        }

        public class Parent
        {
            public Child[] Children { get; set; }
        }

        public class SimpleChildProjection : Projection<Child>
        {
            public SimpleChildProjection()
                : base(DisplayFormatting.RawValues)
            {
                Value(x => x.Name).Name("name");
            }
        }

        public class Child
        {
            public string Name { get; set; }
        }
    }

    public class XmlNodeCentricMediaNode : XmlMediaNode
    {
        public XmlNodeCentricMediaNode(XmlElement element)
            : base(element)
        {
        }

        public static XmlNodeCentricMediaNode ForRoot(string rootElement)
        {
            return new XmlNodeCentricMediaNode(new XmlDocument().WithRoot(rootElement));
        }

        protected override IXmlMediaNode buildChildFor(XmlElement childElement)
        {
            return new XmlNodeCentricMediaNode(childElement);
        }

        public override void SetAttribute(string name, object value)
        {
            if (value != null) Element.AddElement(name).InnerText = value.ToString();
        }
    }

    public abstract class XmlMediaNode : IXmlMediaNode
    {
        private readonly XmlElement _element;

        protected XmlMediaNode(XmlElement element)
        {
            _element = element;
            LinkWriter = AtomXmlLinkWriter.Flyweight;
        }

        public IMediaNode AddChild(string name)
        {
            var childElement = _element.AddElement(name);
            var childNode = buildChildFor(childElement);
            childNode.LinkWriter = LinkWriter;

            return childNode;
        }

        public abstract void SetAttribute(string name, object value);

        public void WriteLinks(IEnumerable<Link> links)
        {
            LinkWriter.Write(_element, links);
        }

        public IMediaNodeList AddList(string nodeName, string leafName)
        {
            var parentElement = _element.AddElement(nodeName);
            return new XmlMediaNodeList(this, parentElement, leafName);
        }

        public XmlElement Element
        {
            get { return _element; }
        }

        public IXmlLinkWriter LinkWriter { get; set; }
        protected abstract IXmlMediaNode buildChildFor(XmlElement childElement);

        public override string ToString()
        {
            return _element.OuterXml;
        }

        public class XmlMediaNodeList : IMediaNodeList
        {
            private readonly XmlMediaNode _parentNode;
            private readonly XmlElement _parentElement;
            private readonly string _childElementName;

            public XmlMediaNodeList(XmlMediaNode parentNode, XmlElement parentElement, string childElementName)
            {
                _parentNode = parentNode;
                _parentElement = parentElement;
                _childElementName = childElementName;
            }

            public IMediaNode Add()
            {
                var element = _parentElement.AddElement(_childElementName);
                return _parentNode.buildChildFor(element);
            }
        }
    }

    public interface IXmlMediaNode : IMediaNode
    {
        XmlElement Element { get; }
        IXmlLinkWriter LinkWriter { get; set; }
    }

    public interface IXmlLinkWriter
    {
        void Write(XmlElement parent, IEnumerable<Link> links);
    }

    public class AtomXmlLinkWriter : IXmlLinkWriter
    {
        public static readonly AtomXmlLinkWriter Flyweight = new AtomXmlLinkWriter();

        public void Write(XmlElement parent, IEnumerable<Link> links)
        {
            links.Each(x => WriteLink(parent, x));
        }

        public void WriteLink(XmlElement parent, Link link)
        {
            var element = parent.AddElement("link").WithAtt("href", link.Url);


            link.Title.IfNotNull(x => element.WithAtt("title", x));
            link.Rel.IfNotNull(x => element.WithAtt("rel", x));
            link.ContentType.IfNotNull(x => element.WithAtt("type", x));
        }
    }
}