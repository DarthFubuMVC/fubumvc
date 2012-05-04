using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Core.View;

namespace FubuMVC.Core.Resources.Conneg
{
    public class OutputNode : BehaviorNode, IMayHaveResourceType
    {
        private readonly WriterChain _chain = new WriterChain();
        private readonly Type _resourceType;

        public OutputNode(Type resourceType)
        {
            _resourceType = resourceType;
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Output; }
        }

        public WriterChain Writers
        {
            get { return _chain; }
        }

        public Type ResourceType
        {
            get { return _resourceType; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var def = new ObjectDef(typeof (OutputBehavior<>), _resourceType);

            var mediaType = typeof (IMedia<>).MakeGenericType(_resourceType);
            var enumerableType = typeof (IEnumerable<>).MakeGenericType(mediaType);
            var dependency = new ListDependency(enumerableType);
            dependency.AddRange(Writers.OfType<IContainerModel>().Select(x => x.ToObjectDef(DiagnosticLevel.None)));

            def.Dependency(dependency);

            return def;
        }

        public WriteWithFormatter AddFormatter<T>() where T : IFormatter
        {

            var formatter = new WriteWithFormatter(_resourceType, typeof (T));
            var existing = Writers.FirstOrDefault(x => x.Equals(formatter));
            if (existing != null)
            {
                return existing as WriteWithFormatter;
            }
            

            Writers.AddToEnd(formatter);

            return formatter;
        }

        public Writer AddWriter<T>()
        {
            var writerType = typeof (IMediaWriter<>).MakeGenericType(_resourceType);
            if (!typeof(T).CanBeCastTo(writerType))
            {
                throw new ArgumentOutOfRangeException("{0} can not be case to {1}", typeof(T).FullName, writerType.FullName);
            }

            var writer = new Writer(typeof (T));

            Writers.AddToEnd(writer);

            return writer;
        }

        public WriteHtml AddHtml()
        {
            var existing = Writers.OfType<WriteHtml>().FirstOrDefault();
            if (existing != null)
            {
                return existing;
            }

            var write = new WriteHtml(_resourceType);
            Writers.AddToEnd(write);

            return write;
        }

        public ViewNode AddView(IViewToken view, Type conditionType = null)
        {
            var node = new ViewNode(view);
            if (conditionType != null && conditionType != typeof(Always))
            {
                node.Condition(conditionType);
            }

            Writers.AddToEnd(node);

            return node;
        }

        public void ClearAll()
        {
            Writers.SetTop(null);
        }

        public void JsonOnly()
        {
            ClearAll();
            AddFormatter<JsonFormatter>();
        }

        public bool UsesFormatter<T>()
        {
            return Writers.OfType<WriteWithFormatter>().Any(x => x.FormatterType == typeof(T));
        }

        public void AddWriter(Type writerType)
        {
            var writer = new Writer(writerType, _resourceType);
            Writers.AddToEnd(writer);
        }

        Type IMayHaveResourceType.ResourceType()
        {
            return ResourceType;
        }

        public bool HasView(Type conditionalType)
        {
            return Writers.OfType<ViewNode>().Any(x => x.ConditionType == conditionalType);
        }

        public override string ToString()
        {
            return Writers.Select(x => x.ToString()).Join(", ");
        }

        public override string Description
        {
            get { return ToString(); }
        }
    }
}