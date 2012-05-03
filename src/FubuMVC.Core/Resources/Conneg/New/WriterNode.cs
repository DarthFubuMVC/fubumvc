using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public abstract class WriterNode : Node<WriterNode, WriterChain>, IContainerModel
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

        ObjectDef IContainerModel.ToObjectDef(DiagnosticLevel diagnosticLevel)
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
            Trace(new ConditionAdded(description));
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
            var description = "By Service:  Func<{0}, bool>".ToFormat(typeof(T).Name);
            Trace(new ConditionAdded(description));
            _conditionalDef = ConditionalObjectDef.ForService(condition);
        }

        /// <summary>
        ///   Makes the behavior execute only if the condition against a model
        ///   object pulled from IFubuRequest is true
        /// </summary>
        public void ConditionByModel<T>(Func<T, bool> filter) where T : class
        {
            var description = "By Model:  Func<{0}, bool>".ToFormat(typeof(T).Name);
            Trace(new ConditionAdded(description));
            _conditionalDef = ConditionalObjectDef.ForModel(filter);
        }

        /// <summary>
        ///   Makes the behavior execute only if the custom IConditional evaluates
        ///   true
        /// </summary>
        public void Condition<T>() where T : IConditional
        {
            Trace(new ConditionAdded(typeof(T)));
            _conditionalDef = ConditionalObjectDef.For<T>();
        }

        public void Condition(Type type)
        {
            Trace(new ConditionAdded(type));
            _conditionalDef = ConditionalObjectDef.For(type);
        }

        public abstract IEnumerable<string> Mimetypes { get; }
    }
}