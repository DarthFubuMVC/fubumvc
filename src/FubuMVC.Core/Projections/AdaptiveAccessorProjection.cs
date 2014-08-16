using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;

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
            runner.Project(context, childNode);

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
        void Project<T>(IProjectionContext<T> context, IMediaNode childNode);
    }

    public class ChildRunner<TValue> : IChildRunner
    {
        private readonly TValue _value;

        public ChildRunner(TValue value)
        {
            _value = value;
        }

        public void Project<T>(IProjectionContext<T> context, IMediaNode childNode)
        {
            var connegGraph = context.Service<ConnegSettings>().Graph;
            var projectionType = connegGraph.WriterTypesFor(typeof (TValue))
                .FirstOrDefault(x => x.CanBeCastTo<IProjection<TValue>>() && x.IsConcreteWithDefaultCtor());



            if (projectionType == null)
            {
                throw new Exception("There is no projection available for type " + typeof(TValue).GetFullName());
            }

            var projection = Activator.CreateInstance(projectionType).As<IProjection<TValue>>();

            var childContext = context.ContextFor(_value);
            projection.Write(childContext, childNode);
        }
    }
}