using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.JqGrid;
using FubuLocalization;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace FubuFastPack.Querying
{
    public interface IQueryService
    {
        IEnumerable<StringToken> FilterOptionsFor<TEntity>(Accessor accessor);
        Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request);
    }

    // TODO -- need to register this
    public interface IFilterTagWriter
    {
        HtmlTag FilterTemplatesFor(ISmartGrid grid);
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
        private readonly IEnumerable<IFilterTemplateSource> _sources;

        public FilterTagWriter(IQueryService queryService, IEnumerable<IFilterTemplateSource> sources)
        {
            _queryService = queryService;
            _sources = sources;
        }

        // TODO -- might need to do default criteria here
        /*
         * Push in:
         * 1.) grid type
         * 2.) FilteredProperty*
         * 3.) initial criteria
         * 
         * 
         * 
         * 
         */
        public HtmlTag FilterTemplatesFor(ISmartGrid grid)
        {
            var tag = new HtmlTag("div");
            var containerNameForGrid = grid.GetType().ContainerNameForGrid();
            tag.Id("filters_" + containerNameForGrid);
            tag.AddClass("smart-grid-filter");
            tag.Child(new TableTag());

            var metadata = new Dictionary<string, object>{
                {"gridId", containerNameForGrid}
            };

            tag.MetaData("filters", metadata);
            
            // Might get FilteredProperty's out of here
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