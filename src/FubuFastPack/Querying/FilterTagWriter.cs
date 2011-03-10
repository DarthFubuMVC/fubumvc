using System;
using System.Collections.Generic;
using System.Linq;
using FubuFastPack.JqGrid;
using HtmlTags;

namespace FubuFastPack.Querying
{
    // TODO -- StructureMap needs to support the Func<T, string> thingie
    // TODO -- UT this
    public class FilterTagWriter
    {
        private readonly IEnumerable<IFilterTemplateSource> _sources;

        public FilterTagWriter(IEnumerable<IFilterTemplateSource> sources)
        {
            _sources = sources;
        }


        public HtmlTag FilterTemplatesFor(GridViewModel model)
        {
            var tag = new HtmlTag("div");
            var containerNameForGrid = model.GridType.ContainerNameForGrid();
            tag.Id("filters_" + containerNameForGrid);
            tag.AddClass("smart-grid-filter");
            tag.Append(new TableTag());

            var metadata = new Dictionary<string, object>{
                {"gridId", "grid_" + model.GridName},
                {"initialCriteria", model.InitialCriteria()}
            };

            tag.MetaData("filters", metadata);

            var properties = model.FilteredProperties;

            var templates = _sources.Distinct().SelectMany(x => x.TagsFor(properties));

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
                div.Add("div").AddClass("smart-grid-editors").Append(templates);
                div.Add("div").AddClass("smart-grid-operators").Append(operators);

                div.Append(new SelectTag(select =>
                {
                    select.AddClass("smart-grid-properties");
                    properties.Each(prop => select.Option(prop.Header, prop.Accessor.Name));
                }));
            });

            return tag;
        }
    }
}