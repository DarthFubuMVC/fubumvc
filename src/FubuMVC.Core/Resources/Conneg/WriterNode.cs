using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Resources.Conneg
{
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
        ///   Make the behavior *only* execute if the condition is met
        /// </summary>
        /// <param name = "condition"></param>
        /// <param name="description"></param>
        public void Condition(Func<bool> condition, string description = "Anonymous")
        {
            _conditionalDef = ConditionalObjectDef.For(condition);
        }

        /// <summary>
        ///   Makes the behavior execute only if the condition against a service
        ///   in the underlying IoC container is true
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "condition"></param>
        public void ConditionByService<T>(Func<T, bool> condition)
        {
            _conditionalDef = ConditionalObjectDef.ForService(condition);
        }

        /// <summary>
        ///   Makes the behavior execute only if the condition against a model
        ///   object pulled from IFubuRequest is true
        /// </summary>
        public void ConditionByModel<T>(Func<T, bool> filter) where T : class
        {
            _conditionalDef = ConditionalObjectDef.ForModel(filter);
        }

        /// <summary>
        ///   Makes the behavior execute only if the custom IConditional evaluates
        ///   true
        /// </summary>
        public void Condition<T>() where T : IConditional
        {
            _conditionalDef = ConditionalObjectDef.For<T>();
        }

        public void Condition(Type type)
        {
            _conditionalDef = ConditionalObjectDef.For(type);
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