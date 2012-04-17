using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Media.Projections
{

    public class EnumerableProjection<TParent, TChild> : IProjection<TParent>
    {
        public Func<IProjectionContext<TParent>, IProjection<TChild>> ProjectionSource;
        public Func<IProjectionContext<TParent>, IEnumerable<TChild>> ElementSource;
        public string NodeName;
        public string LeafName = typeof(TChild).Name;

        public static EnumerableProjection<TParent, TChild> For(Expression<Func<TParent, IEnumerable<TChild>>> expression)
        {
            var accessor = ReflectionHelper.GetAccessor(expression);

            return new EnumerableProjection<TParent, TChild>{
                ElementSource = c => c.ValueFor(accessor).As<IEnumerable<TChild>>(),
                NodeName = accessor.Name
            };
        }

        public void UseProjection<TProjection>() where TProjection : IProjection<TChild>
        {
            ProjectionSource = c => c.Service<TProjection>();
        }

        public void DefineProjection(Action<Projection<TChild>> configure)
        {
            ProjectionSource = c =>
            {
                var projection = new Projection<TChild>(DisplayFormatting.RawValues);
                configure(projection);

                return projection;
            };
        }

        public void Write(IProjectionContext<TParent> context, IMediaNode node)
        {
            var list = node.AddList(NodeName, LeafName ?? NodeName);
            var elements = ElementSource(context);

            var projection = ProjectionSource(context);
            elements.Each(element =>
            {
                var childNode = list.Add();
                var childContext = context.ContextFor(element);
                projection.Write(childContext, childNode);
            });
        }




    }

}