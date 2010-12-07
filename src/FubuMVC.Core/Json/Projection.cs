using System.Collections.Generic;
using FubuCore.Util;

namespace FubuMVC.Core.Json
{
    public class Projection<T> : IProjection
    {
        private readonly IEnumerable<IValueProjection<T>> _values;

        public Projection(IEnumerable<IValueProjection<T>> values)
        {
            _values = values;
        }

        public Cache<string, object> Flatten(object target)
        {
            var strongTarget = (T) target;
            var props = new Cache<string, object>();

            _values.Each(v => v.Map(strongTarget, props));

            return props;
        }
    }
}