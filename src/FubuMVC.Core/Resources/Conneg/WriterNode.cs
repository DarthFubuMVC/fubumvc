using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Resources.Conneg
{
    [MarkedForTermination]
    public abstract class WriterNode : Node<WriterNode, WriterChain>, IContainerModel, DescribesItself
    {
        private static readonly ObjectDef _always = ObjectDef.ForValue(Always.Flyweight);

        private ObjectDef _conditionalDef = _always;
        

        public abstract Type ResourceType { get; }

        public Type ConditionType
        {
            get
            {
                return HasCondition()
                    ? _conditionalDef.Type
                    : typeof(Always);
            }
        }

        ObjectDef IContainerModel.ToObjectDef()
        {
            var def = new ObjectDef(typeof (Media<>), ResourceType);
            def.DependencyByType<IConditional>(_conditionalDef);

            var writerType = typeof (IMediaWriter<>).MakeGenericType(ResourceType);
            
            // TODO -- validate that it's really an IMediaWriter<T>
            def.Dependency(writerType, toWriterDef());

            return def;
        }

        public bool HasCondition()
        {
            return !ReferenceEquals(_always, _conditionalDef);
        }

        protected abstract ObjectDef toWriterDef();

        /// <summary>
        ///   Makes the behavior execute only if the custom IConditional evaluates
        ///   true
        /// </summary>
        public void Condition<T>() where T : IConditional
        {
            _conditionalDef = ObjectDef.ForType<T>();
        }

        public void Condition(Type type)
        {
            _conditionalDef = new ObjectDef(type);
        }

        public abstract IEnumerable<string> Mimetypes { get; }

        void DescribesItself.Describe(Description description)
        {
            createDescription(description);
            description.Properties["Condition"] = _conditionalDef.ToDescriptionText();
        }

        protected abstract void createDescription(Description description);
    }
}