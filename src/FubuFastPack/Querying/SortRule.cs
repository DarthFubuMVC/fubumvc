using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;

namespace FubuFastPack.Querying
{
    public class SortRule<T> where T : DomainEntity
    {
        private bool _ascending = true;
        private string _fieldName = "Id";

        private SortRule() { }

        public static SortRule<T> Ascending(Expression<Func<T, object>> property)
        {
            string fieldName = ReflectionHelper.GetAccessor(property).Name;
            return new SortRule<T>() { _ascending = true, _fieldName = fieldName };
        }

        public static SortRule<T> Descending(Expression<Func<T, object>> property)
        {
            string fieldName = ReflectionHelper.GetProperty(property).Name;
            return new SortRule<T>() { _ascending = false, _fieldName = fieldName };
        }

        public void ApplyDefaultSorting(GridDataRequest paging)
        {
            if (paging.SortColumn.IsNotEmpty()) return;

            paging.SortColumn = _fieldName;
            paging.SortAscending = _ascending;
        }

        public string FieldName { get { return _fieldName; } }
        public bool IsAscending { get { return _ascending; } }
    }
}