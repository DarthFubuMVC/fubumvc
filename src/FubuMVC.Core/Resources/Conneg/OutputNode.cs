using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public class OutputNode : BehaviorNode, IMayHaveResourceType, DescribesItself, ICompositeTracedModel
    {
        private readonly WriterChain _chain = new WriterChain();
        private readonly Type _resourceType;

        public OutputNode(Type resourceType)
        {
            if (resourceType == typeof (void))
            {
                throw new ArgumentOutOfRangeException("Void is not a valid resource type");
            }

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

        public override string Description
        {
            get { return ToString(); }
        }

        /// <summary>
        /// Use this if you want to override the handling for 
        /// the resource not being found on a chain by chain
        /// basis
        /// </summary>
        public ObjectDef ResourceNotFound { get; set; }

        /// <summary>
        /// Use the specified type T as the resource not found handler strategy
        /// for only this chain
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UseForResourceNotFound<T>() where T : IResourceNotFoundHandler
        {
            ResourceNotFound = ObjectDef.ForType<T>();
        }

        #region DescribesItself Members

        void DescribesItself.Describe(Description description)
        {
            description.Title = "OutputNode";
            description.ShortDescription = "Render the output for resource " + ResourceType.Name;

            description.AddList("Writers", Writers);
        }

        #endregion

        #region IMayHaveResourceType Members

        Type IMayHaveResourceType.ResourceType()
        {
            return ResourceType;
        }

        #endregion

        protected override ObjectDef buildObjectDef()
        {
            var def = new ObjectDef(typeof (OutputBehavior<>), _resourceType);

            Type mediaType = typeof (IMedia<>).MakeGenericType(_resourceType);
            Type enumerableType = typeof (IEnumerable<>).MakeGenericType(mediaType);
            var dependency = new ListDependency(enumerableType);
            dependency.AddRange(Writers.OfType<IContainerModel>().Select(x => x.ToObjectDef()));

            def.Dependency(dependency);

            if (ResourceNotFound != null)
            {
                def.Dependency(typeof(IResourceNotFoundHandler), ResourceNotFound);
            }

            return def;
        }

        public WriteWithFormatter AddFormatter<T>() where T : IFormatter
        {
            var formatter = new WriteWithFormatter(_resourceType, typeof (T));
            WriterNode existing = Writers.FirstOrDefault(x => x.Equals(formatter));
            if (existing != null)
            {
                return existing as WriteWithFormatter;
            }


            Writers.AddToEnd(formatter);

            return formatter;
        }

        public Writer AddWriter<T>()
        {
            Type writerType = typeof (IMediaWriter<>).MakeGenericType(_resourceType);
            if (!typeof (T).CanBeCastTo(writerType))
            {
                throw new ArgumentOutOfRangeException("{0} cannot be cast to {1}".ToFormat(typeof (T).FullName,
                                                                                           writerType.FullName));
            }

            var writer = new Writer(typeof (T));

            Writers.AddToEnd(writer);

            return writer;
        }

        public WriteHtml AddHtml()
        {
            WriteHtml existing = Writers.OfType<WriteHtml>().FirstOrDefault();
            if (existing != null)
            {
                return existing;
            }

            var write = new WriteHtml(_resourceType);
            Writers.AddToEnd(write);

            return write;
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
            return Writers.OfType<WriteWithFormatter>().Any(x => x.FormatterType == typeof (T));
        }

        public void AddWriter(Type writerType)
        {
            var writer = new Writer(writerType, _resourceType);
            Writers.AddToEnd(writer);
        }

        public override string ToString()
        {
            return Writers.Select(x => x.ToString()).Join(", ");
        }

        IEnumerable<ITracedModel> ICompositeTracedModel.Children
        {
            get { return _chain; }
        }
    }
}