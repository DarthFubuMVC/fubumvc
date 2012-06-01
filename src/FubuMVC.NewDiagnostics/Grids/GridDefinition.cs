using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using HtmlTags.Extended.Attributes;
using FubuMVC.Core.UI;

namespace FubuMVC.NewDiagnostics.Grids
{
    public static class GridFubuPageExtensions
    {
        public static HtmlTag RenderGrid<T>(this IFubuPage page, string id) where T : IGridDefinition, new()
        {
            var grid = new T();

            var div = new HtmlTag("div").Id(id).AddClass("slick-grid");
            div.Data("columns", grid.ToColumnJson());

            page.Asset("diagnostics/SlickGridActivator.js");


            var url = grid.SelectDataSourceUrl(page.Urls);
            if (url.IsNotEmpty())
            {
                div.Data("url", url);
            }

            return div;
        } 
    }

    public class Try
    {
        public void Do()
        {
            var grid = new BehaviorChainGrid();
            var registry = new FubuRegistry();
            grid.As<IFubuRegistryExtension>().Configure(registry);


            registry.BuildGraph();
        }
    }

    public class BehaviorChainSource : IGridDataSource<BehaviorChain>
    {
        private readonly BehaviorGraph _graph;

        public BehaviorChainSource(BehaviorGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<BehaviorChain> GetData()
        {
            return _graph.Behaviors;
        }
    }

    public class BehaviorChainGrid : GridDefinition<BehaviorChain>
    {
        public BehaviorChainGrid()
        {
            SourceIs<BehaviorChainSource>();

            Column(x => x.UniqueId);
            Column(x => x.FirstCall().Description);
        }
    }

    public static class DiagnosticConstants
    {
        public static readonly string UrlPrefix = "_diagnostics";

        public static readonly string GetDataMethodName =
            ReflectionHelper.GetMethod<IGridDataSource<BehaviorChain>>(x => x.GetData()).Name;
    }

    public interface IGridDefinition
    {
        string ToColumnJson();
        string SelectDataSourceUrl(IUrlRegistry urls);
    }

    public interface IGridDefinition<T> : IGridDefinition
    {
        IEnumerable<IDictionary<string, object>> FormatData(IEnumerable<T> data);
    }

    public interface IGridColumn<T>
    {
        void WriteColumn(StringBuilder builder);
        void WriteField(T target, IDictionary<string, object> dictionary);
    }

    public abstract class GridDefinition<T> : IGridDefinition<T>, IFubuRegistryExtension
    {
        private readonly IList<IGridColumn<T>> _columns = new List<IGridColumn<T>>();
        private Type _queryType;
        private Type _sourceType;

        /// <summary>
        /// Source type must implement either IGridDataSource<T> or IGridDataSource<T, TQuery>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        public void SourceIs<TSource>()
        {
            var sourceType = typeof (TSource);
            var templateType = sourceType.FindInterfaceThatCloses(typeof(IGridDataSource<>));
            if (templateType != null)
            {
                if (templateType.GetGenericArguments().First() != typeof(T))
                {
                    throw new ArgumentOutOfRangeException("Wrong type as the argument to IGridDataSource<>");
                }

                _queryType = null;
                _sourceType = sourceType;

                return;
            }

            templateType = sourceType.FindInterfaceThatCloses(typeof(IGridDataSource<,>));
            if (templateType != null)
            {
                if (templateType.GetGenericArguments().First() != typeof(T))
                {
                    throw new ArgumentOutOfRangeException("Wrong type as the argument to IGridDataSource<>");
                }

                _queryType = templateType.GetGenericArguments().Last();
                _sourceType = sourceType;

                return;
            }

            throw new ArgumentOutOfRangeException("TSource must be either IGridDataSource<T> or IGridDataSource<TQuery>");
        }

        string IGridDefinition.ToColumnJson()
        {
            var builder = new StringBuilder();
            builder.Append("[");

            for (var i = 0; i < _columns.Count - 1; i++)
            {
                var column = _columns[i];
                column.WriteColumn(builder);
                builder.Append(", ");
            }

            _columns.Last().WriteColumn(builder);

            builder.Append("]");

            return builder.ToString();
        }

        string IGridDefinition.SelectDataSourceUrl(IUrlRegistry urls)
        {
            if (_sourceType == null) return null;

            if (_queryType != null)
            {
                return urls.UrlFor(_queryType);
            }

            var runnerType = createRunnerType();

            return urls.UrlFor(runnerType);
        }

        public Type SourceType
        {
            get { return _sourceType; }
        }

        IEnumerable<IDictionary<string, object>> IGridDefinition<T>.FormatData(IEnumerable<T> data)
        {
            return data.Select(x =>
            {
                var dictionary = new Dictionary<string, object>();
                _columns.Each(col => col.WriteField(x, dictionary));

                return dictionary;
            });
        }

        public ColumnDefinition<T, TProp> Column<TProp>(Expression<Func<T, TProp>> property)
        {
            var column = new ColumnDefinition<T, TProp>(property);
            _columns.Add(column);

            return column;
        }

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Configure(graph =>
            {
                Type runnerType = createRunnerType();


                var method = runnerType.GetMethod("Run");

                var call = new ActionCall(runnerType, method);
                var chain = new BehaviorChain();
                chain.AddToEnd(call);
                chain.Route = new RouteDefinition(DiagnosticConstants.UrlPrefix);
                chain.Route.Append("_data");
                chain.Route.Append(typeof(T).Name);

                chain.MakeAsymmetricJson();

                graph.AddChain(chain);

            });
        }

        private Type createRunnerType()
        {
            return _queryType == null
                       ? typeof (GridRunner<,,>).MakeGenericType(typeof (T), GetType(), _sourceType)
                       : typeof (GridRunner<,,,>).MakeGenericType(typeof (T), GetType(), _sourceType, _queryType);
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
                builder.Append(key);
                builder.Append(": ");

                if (value is string)
                {
                    builder.Append("\"");
                    builder.Append(value as string);
                    builder.Append("\"");
                }
                else if (value is bool)
                {
                    builder.Append(value.ToString().ToLower());
                }
                else
                {
                    builder.Append(value.ToString());
                }

                builder.Append(", ");
            });

            builder.Remove(builder.Length - 2, 2);
            builder.Append("}");
        }

        void IGridColumn<T>.WriteField(T target, IDictionary<string, object> dictionary)
        {
            var rawValue = _accessor.GetValue(target);

            // TODO -- this'll get fancier later
            dictionary.Add(_cache["field"].As<string>(), rawValue);
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
    }
}