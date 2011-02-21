using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Reflection;
using NHibernate;
using FubuFastPack.Domain;

namespace FubuFastPack.JqGrid
{

    public class EntityGridData : IGridData
    {
        private readonly Queue<object> _records;
        public object _currentRow;

        public EntityGridData(IEnumerable<object> data)
        {
            _records = new Queue<object>(data);
        }

        public Func<object> GetterFor(Accessor accessor)
        {
            return () => accessor.GetValue(_currentRow);
        }

        public bool MoveNext()
        {
            if (_records.Any())
            {
                _currentRow = _records.Dequeue();
                return true;
            }

            return false;
        }

        public object CurrentRowType()
        {
            return _currentRow.GetTrueType();
        }
    }


    public class ProjectionGridData<T> : IGridData
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
                _currentRow = (object[])_records.Dequeue();
                return true;
            }

            return false;
        }

        public object CurrentRowType()
        {
            return _currentRow.Last(); 
        }
    }
}