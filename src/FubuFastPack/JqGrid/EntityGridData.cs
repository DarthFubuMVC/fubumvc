using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuFastPack.Domain;

namespace FubuFastPack.JqGrid
{
    public class EntityGridData<T> : IGridData
    {
        private T _current;
        private readonly Queue<T> _queue;

        public EntityGridData(IEnumerable<T> entities)
        {
            _queue = new Queue<T>(entities);
        }

        public Func<object> GetterFor(Accessor accessor)
        {
            return () => accessor.GetValue(_current);
        }

        public bool MoveNext()
        {
            if (_queue.Any())
            {
                _current = _queue.Dequeue();
                return true;
            }

            return false;
        }

        public object CurrentRowType()
        {
            return _current.GetTrueType();
        }
    }
}