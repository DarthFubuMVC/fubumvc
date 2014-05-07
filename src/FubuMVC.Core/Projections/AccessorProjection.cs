using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Projections
{
    public interface IAccessorProjection
    {
        void ApplyNaming(IAccessorNaming naming);
    }

    public class AccessorProjection<T, TValue> : IProjection<T>, IAccessorProjection
    {
        private readonly Accessor _accessor;
        private ISingleValueProjection<T> _inner;

        public AccessorProjection(Accessor accessor)
        {
            _accessor = accessor;

            if (typeof(TValue).CanBeCastTo<IProjectMyself>())
            {
                _inner = typeof (SelfProjectingValueProjector<,>)
                    .CloseAndBuildAs<ISingleValueProjection<T>>(accessor, typeof (T), typeof (TValue));
            }
            else
            {
                _inner = new SingleValueProjection<T>(_accessor.Name, c => c.Values.ValueFor(_accessor));
            }
        }

        void IAccessorProjection.ApplyNaming(IAccessorNaming naming)
        {
            _inner.AttributeName = naming.Name(_accessor);
        }

        /// <summary>
        /// Use an IValueProjector strategy to override how the projection is done for a complex object
        /// </summary>
        /// <typeparam name="TProjector"></typeparam>
        /// <returns></returns>
        public AccessorProjection<T, TValue> ProjectWith<TProjector>() where TProjector : IValueProjector<TValue>, new()
        {
            return ProjectWith(new TProjector());
        } 

        /// <summary>
        /// Use an IValueProjector strategy to override how the projection is performed for a complex object
        /// </summary>
        /// <param name="projector"></param>
        /// <returns></returns>
        public AccessorProjection<T, TValue> ProjectWith(IValueProjector<TValue> projector)
        {
            _inner = new ExternallyFormattedValueProjector<T, TValue>(_accessor, projector)
            {
                AttributeName = Name()
            };

            return this;

        } 

        /// <summary>
        /// Helper method to build AccessorProjection objects individually
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static AccessorProjection<T, TValue> For(Expression<Func<T, TValue>> expression)
        {
            return new AccessorProjection<T, TValue>(ReflectionHelper.GetAccessor(expression));
        }

        /// <summary>
        /// Overrides the attribute name in the projection for this accessor
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public AccessorProjection<T, TValue> Name(string value)
        {
            _inner.AttributeName = value;
            return this;
        }

        /// <summary>
        /// Applies the IDisplayFormatter formatting to this individual 
        /// </summary>
        /// <returns></returns>
        public AccessorProjection<T, TValue> Formatted()
        {
            _inner = new SingleValueProjection<T>(_inner.AttributeName, context => context.FormattedValueOf(_accessor));

            return this;
        }

        /// <summary>
        /// Use a Func to transform the property value to another value in the projection
        /// </summary>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public AccessorProjection<T, TValue> FormattedBy(Func<TValue, object> formatting)
        {
            _inner = new SingleValueProjection<T>(_inner.AttributeName, context =>
            {
                var raw = context.Values.ValueFor(_accessor);
                if (raw == null)
                {
                    return null;
                }

                return formatting((TValue)raw);
            });

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputBuilder"></param>
        /// <returns></returns>
        public AccessorProjection<T, TValue> WriteUrlFor(Func<TValue, object> inputBuilder)
        {
            return WriteUrlFor((urls, value) =>
            {
                var inputModel = inputBuilder(value);
                return urls.UrlFor(inputModel);
            });
        }

        /// <summary>
        /// Writes the Url that accesses the value of this accessor as the input
        /// </summary>
        /// <param name="urlFinder"></param>
        /// <returns></returns>
        public AccessorProjection<T, TValue> WriteUrlFor(Func<IUrlRegistry, TValue, string> urlFinder)
        {
            _inner = new SingleValueProjection<T>(_inner.AttributeName, context =>
            {
                var raw = context.Values.ValueFor(_accessor);
                if (raw == null)
                {
                    return string.Empty;
                }

                return urlFinder(context.Urls, (TValue)raw);
            });

            return this;
        }

        /// <summary>
        /// Retrieve the attribute name used in the projection
        /// </summary>
        /// <returns></returns>
        public string Name()
        {
            return _inner.AttributeName;
        }

        void IProjection<T>.Write(IProjectionContext<T> context, IMediaNode node)
        {
            _inner.Write(context, node);
        }

        IEnumerable<Accessor> IProjection<T>.Accessors()
        {
            yield return _accessor;
        }
    }


}