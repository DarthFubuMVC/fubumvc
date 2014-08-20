using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Projections
{
    public class AdaptiveAccessorProjection<T> : IProjection<T>, IAccessorProjection
    {
        private readonly Accessor _accessor;
        private string _name;

        public AdaptiveAccessorProjection(Accessor accessor)
        {
            _accessor = accessor;
            _name = _accessor.Name;
        }

        public void Write(IProjectionContext<T> context, IMediaNode node)
        {
            var value = context.Values.ValueFor(_accessor);
            if (value == null)
            {
                return;
            }

            var childNode = node.AddChild(_name);
            var runner = typeof (ChildRunner<>).CloseAndBuildAs<IChildRunner>(value, value.GetType());
            runner.Project(context, childNode, node, _name);

        }

        public IEnumerable<Accessor> Accessors()
        {
            yield return _accessor;
        }

        void IAccessorProjection.ApplyNaming(IAccessorNaming naming)
        {
            _name = naming.Name(_accessor);
        }

        public AdaptiveAccessorProjection<T> Name(string name)
        {
            _name = name;
            return this;
        }


    }

    public interface IChildRunner
    {
        void Project<T>(IProjectionContext<T> context, IMediaNode childNode, IMediaNode parentNode, string nodeName);
    }

    public class ChildRunner<TValue> : IChildRunner
    {
        private readonly TValue _value;

        public ChildRunner(TValue value)
        {
            _value = value;
        }

        public void Project<T>(IProjectionContext<T> context, IMediaNode childNode, IMediaNode parentNode, string nodeName)
        {
            var connegGraph = context.Service<ConnegSettings>().Graph;
            var projectionType = connegGraph.WriterTypesFor(typeof (TValue))
                .FirstOrDefault(x => x.CanBeCastTo<IProjection<TValue>>() && x.IsConcreteWithDefaultCtor());

            var childContext = context.ContextFor(_value);

            if (projectionType == null)
            {
                parentNode.SetAttribute(nodeName, _value);
            }
            else
            {
                var projection = Activator.CreateInstance(projectionType).As<IProjection<TValue>>();
                projection.Write(childContext, childNode);
            }

        }
    }
}