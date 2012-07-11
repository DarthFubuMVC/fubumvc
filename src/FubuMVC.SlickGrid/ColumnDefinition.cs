using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuMVC.SlickGrid
{
    public class SlickGridFormatter
    {
        public static readonly SlickGridFormatter TypeFormatter = new SlickGridFormatter("Slick.Formatters.DotNetType");
        public static readonly SlickGridFormatter StringArray = new SlickGridFormatter("Slick.Formatters.StringArray");

        private readonly string _name;

        public SlickGridFormatter(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    public class ColumnDefinition<T, TProp> : IGridColumn<T>
    {
        private readonly Accessor _accessor;
        private readonly Cache<string, object> _cache;

        public ColumnDefinition(Expression<Func<T, TProp>> property)
        {
            _cache = new Cache<string, object>();

            _accessor = ReflectionHelper.GetAccessor(property);

            Title(_accessor.Name);
            Field(_accessor.Name);
            Id(_accessor.Name);

            Sortable(true);
        }

        void IGridColumn<T>.WriteColumn(StringBuilder builder)
        {
            builder.Append("{");

            _cache.Each((key, value) =>
            {
                builder.WriteJsonProp(key, value);
                builder.Append(", ");
            });
            

            builder.Remove(builder.Length - 2, 2);
            builder.Append("}");
        }



        void IGridColumn<T>.WriteField(T target, IDictionary<string, object> dictionary)
        {
            var rawValue = _accessor.GetValue(target);

            // TODO -- this'll get fancier later
            dictionary.Add(_cache["field"].As<string>(), JsonValueWriter.ConvertToJson(rawValue));
        }

        /// <summary>
        ///   True by default
        /// </summary>
        /// <param name = "isSortable"></param>
        /// <returns></returns>
        public ColumnDefinition<T, TProp> Sortable(bool isSortable)
        {
            _cache["sortable"] = isSortable;

            return this;
        }

        /// <summary>
        ///   By default, this is the name of the property
        /// </summary>
        /// <param name = "title"></param>
        /// <returns></returns>
        public ColumnDefinition<T, TProp> Title(string title)
        {
            _cache["name"] = title;

            return this;
        }

        /// <summary>
        ///   By default, this is the name of the property
        /// </summary>
        /// <param name = "field"></param>
        /// <returns></returns>
        public ColumnDefinition<T, TProp> Field(string field)
        {
            _cache["field"] = field;
            return this;
        }

        /// <summary>
        ///   By default, this is the name of the property
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        public ColumnDefinition<T, TProp> Id(string id)
        {
            _cache["id"] = id;
            return this;
        }

        public ColumnDefinition<T, TProp> Resizable(bool resizable)
        {
            _cache["resizable"] = resizable;
            return this;
        }

        public ColumnDefinition<T, TProp> Width(int width = 0, int minWidth = 0, int maxWidth = 0)
        {
            if (width > 0)
            {
                _cache["width"] = width;
            }

            if (minWidth > 0)
            {
                _cache["minWidth"] = minWidth;
            }

            if (maxWidth > 0)
            {
                _cache["maxWidth"] = maxWidth;
            }

            return this;
        }

        public ColumnDefinition<T, TProp> Property(string property, object value)
        {
            _cache[property] = value;
            return this;
        }

        public ColumnDefinition<T, TProp> Formatter(string formatter)
        {
            _cache["formatter"] = new SlickGridFormatter(formatter);
            return this;
        }

        public ColumnDefinition<T, TProp> Formatter(SlickGridFormatter formatter)
        {
            _cache["formatter"] = formatter;
            return this;
        }
    }
}