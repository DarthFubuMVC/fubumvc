using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Resources.Media.Formatters;
using FubuMVC.Core.Resources.Media.Projections;
using FubuMVC.Core.Resources.Media.Xml;

namespace FubuMVC.Core.Resources
{
    public class Resource<T> : IResourceRegistration
    {
        private readonly Lazy<LinksSource<T>> _links = new Lazy<LinksSource<T>>(() => new LinksSource<T>());
        private readonly IList<Action<ConnegOutputNode>> _modifications = new List<Action<ConnegOutputNode>>();
        private readonly Lazy<Projection<T>> _projection = new Lazy<Projection<T>>(() => new Projection<T>());

        public Resource()
        {
            modify = node => node.UseNoFormatters();
        }

        private Action<ConnegOutputNode> modify
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
                behaviorGraph.Services.SetServiceIfNone<IValueProjection<T>>(_projection.Value);
            }

            if (_links.IsValueCreated)
            {
                behaviorGraph.Services.SetServiceIfNone<ILinkSource<T>>(_links.Value);
            }
        }

        public void SerializeToXml()
        {
            modify = node => node.UseFormatter<XmlFormatter>();
        }

        public void SerializeToJson()
        {
            modify = node => node.UseFormatter<JsonFormatter>();
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

                node.AddWriter(writerNode);
            };
        }

        public AccessorProjection<T> ProjectValue(Expression<Func<T, object>> expression)
        {
            return _projection.Value.Value(expression);
        }
    }
}