using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.JqGrid;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace FubuFastPack.Querying
{
    public interface IQueryService
    {
        IEnumerable<OperatorKeys> FilterOptionsFor<TEntity>(Accessor accessor);
        Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request);
    }

    // TODO -- need to register this
    public interface IFilterTagWriter
    {
        HtmlTag FilterTemplatesFor<T>() where T : ISmartGrid;
    }

    public interface IFilterTemplateSource
    {
        IEnumerable<HtmlTag> TagsFor(IEnumerable<FilteredProperty> properties);
    }


    // TODO -- StructureMap needs to support the Func<T, string> thingie
    // TODO -- UT this
    public class FilterTagWriter : IFilterTagWriter
    {
        private readonly IQueryService _queryService;
        private readonly IServiceLocator _services;
        private readonly IEnumerable<IFilterTemplateSource> _sources;

        public FilterTagWriter(IQueryService queryService, IServiceLocator services, IEnumerable<IFilterTemplateSource> sources)
        {
            _queryService = queryService;
            _services = services;
            _sources = sources;
        }

        // TODO -- might need to do default criteria here
        public HtmlTag FilterTemplatesFor<T>() where T : ISmartGrid
        {
            var tag = new HtmlTag("div");
            tag.Id("filters_" + typeof (T).ContainerNameForGrid());
            tag.AddClass("smart-grid-filter");
            tag.Child(new TableTag());

            var metadata = new Dictionary<string, object>{
                {"gridId", typeof (T).ContainerNameForGrid()}
            };

            tag.MetaData("filters", metadata);
            
            var grid = _services.GetInstance<T>();
            var properties = grid.AllFilteredProperties(_queryService);
            var templates = _sources.SelectMany(x => x.TagsFor(properties));

            var operators = properties.Select(prop =>
            {
                return new SelectTag(select =>
                {
                    prop.Operators.Each(oper => select.Option(oper.ToString(), oper.Key));
                }).AddClass(prop.Accessor.Name);
            });

            tag.Add("div", div =>
            {
                div.Hide();
                div.AddClass("templates");
                div.Add("div").AddClass("smart-grid-editors").AddChildren(templates);
                div.Add("div").AddClass("smart-grid-operators").AddChildren(operators);

                div.Child(new SelectTag(select =>
                {
                    select.AddClass("smart-grid-properties");
                    properties.Each(prop => select.Option(prop.Header, prop.Accessor.Name));
                }));
            });

            return tag;
        }
    }















}