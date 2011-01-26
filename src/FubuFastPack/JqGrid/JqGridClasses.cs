using System;
using System.Collections;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Querying;
using System.Linq;

namespace FubuFastPack.JqGrid
{
    // Need one for projections, another for entities
    public interface IGridData
    {
        Func<object> GetterFor(Accessor accessor);
        bool MoveNext();
    }

    public class ProjectionGridData : IGridData
    {
        private readonly IList<Accessor> _accessors;
        private readonly Queue<object> _records;
        private object[] _currentRow;

        public ProjectionGridData(IList<object> records, IList<Accessor> accessors)
        {
            _records = new Queue<object>(records);
            _accessors = accessors;
        }

        public Func<object> GetterFor(Accessor accessor)
        {
            var index = _accessors.IndexOf(accessor);
            return () => _currentRow[index];
        }

        public bool MoveNext()
        {
            if (_records.Any())
            {
                _currentRow = (object[]) _records.Dequeue();
                return true;
            }

            return false;
        }
    }

    public class EntityGridData<T> : IGridData
    {
        private T _current;
        private Queue<T> _queue;

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
    }

    // One for projections, one for an entity
    public interface IGridDataSource
    {
        IGridData Fetch(PagingOptions options);
    }





    public class EntityDTO
    {
        private readonly List<string> _cells = new List<string>();
        private readonly Cache<string, string> _properties = new Cache<string, string>();

        public string this[string key]
        {
            get
            {
                return _properties[key];
            }
            set
            {
                _properties[key] = value;
            }
        }

        public object[] cell
        {
            get
            {
                var list = new List<object>();
                list.Add(_properties.ToDictionary());
                list.AddRange(_cells);

                return list.ToArray();
            }
            set
            {
                
            }
        }
    }
}