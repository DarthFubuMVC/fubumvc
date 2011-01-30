using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public class GridRunner
    {
        private readonly IDisplayFormatter _formatter;
        private readonly IUrlRegistry _urls;
        private readonly IServiceLocator _services;

        public GridRunner(IDisplayFormatter formatter, IUrlRegistry urls, IServiceLocator services)
        {
            _formatter = formatter;
            _urls = urls;
            _services = services;
        }

        public GridResults Fetch(PagingOptions paging, IGrid grid)
        {
            var source = grid.BuildSource(_services);
            var data = source.Fetch(paging);
            var actions = grid.Definition.Columns.Select(col => col.CreateFiller(data, _formatter, _urls));

            var list = new List<EntityDTO>();

            while (data.MoveNext())
            {
                var dto = new EntityDTO();
                actions.Each(a => a(dto));

                list.Add(dto);
            }

            // TODO -- needs some UT's
            int recordCount = source.TotalCount();
            var pageCount = (int)Math.Ceiling(recordCount / (double)paging.ResultsPerPage);

            if (pageCount < paging.Page)
            {
                paging.Page = pageCount;
            }

            return new GridResults(){
                page = paging.Page,
                records = recordCount,
                total = pageCount,
                items = list
            };
        }


    }
}