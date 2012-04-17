using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Media.Projections;
using FubuMVC.Media.Xml;

namespace FubuMVC.Media
{
    public class Resource<T> : IResourceRegistration
    {
        private readonly Lazy<LinksSource<T>> _links = new Lazy<LinksSource<T>>(() => new LinksSource<T>());
        private readonly IList<Action<OutputNode>> _modifications = new List<Action<OutputNode>>();
        private readonly Lazy<Projection<T>> _projection = new Lazy<Projection<T>>(() => new Projection<T>(DisplayFormatting.RawValues));

        public Resource()
        {
            modify = node =>
            {
                node.Writers.OfType<WriteWithFormatter>().ToList().Each(x => x.Remove());
            };
        }

        private Action<OutputNode> modify
        {
            set { _modifications.Add(value); }
        }

        public LinksSource<T> Links
        {
            get { return _links.Value; }
        }

        public void Modify(ConnegGraph graph, BehaviorGraph behaviorGraph)
        {
            graph.OutputNodesFor<T>().Each(node => _modifications.Each(x => x(node)));

            if (_projection.IsValueCreated)
            {
                behaviorGraph.Services.SetServiceIfNone(typeof(IProjection<T>), ObjectDef.ForValue(_projection.Value));
            }

            if (_links.IsValueCreated)
            {
                behaviorGraph.Services.SetServiceIfNone(typeof(ILinkSource<T>), ObjectDef.ForValue(_links.Value));
            }
        }

        public void SerializeToXml()
        {
            modify = node => node.AddFormatter<XmlFormatter>();
        }

        public void SerializeToJson()
        {
            modify = node => node.AddFormatter<JsonFormatter>();
        }

        public void WriteToXml(Action<XmlMediaOptions> configure)
        {
            var options = new XmlMediaOptions();
            configure(options);

            modify = node =>
            {
                var writerNode = new MediaWriterNode(typeof (T));
                writerNode.Document.UseType<XmlMediaDocument>().DependencyByValue(options);
                if (_links.IsValueCreated) writerNode.Links.UseValue(_links.Value);
                if (_projection.IsValueCreated) writerNode.Projection.UseValue(_projection.Value);

                node.Writers.AddToEnd(writerNode);
            };
        }

        public AccessorProjection<T, TValue> ProjectValue<TValue>(Expression<Func<T, TValue>> expression)
        {
            return _projection.Value.Value(expression);
        }
    }
}