using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    // Right now, SmartGrid's can *only* have a dependency on 1 Entity in the ctor
    public class SmartGridHarness<T> where T : ISmartGrid
    {
        private readonly Cache<Type, object> _args = new Cache<Type, object>();
        private readonly IFubuRequest _request;
        private readonly IServiceLocator _services;
        private readonly IUrlRegistry _urls;

        public SmartGridHarness(IServiceLocator services, IUrlRegistry urls, IFubuRequest request)
        {
            _services = services;
            _urls = urls;
            _request = request;
            _args.OnMissing = findArg;
        }

        private object findArg(Type type)
        {
            return _request.Get(type);
        }

        private T buildGrid()
        {
            var ctor = typeof (T).GetConstructors().Single();
            var args = ctor.GetParameters().Select(x => _args[x.ParameterType]);

            return (T) Activator.CreateInstance(typeof (T), args.ToArray());
        }

        public void RegisterArgument(Type argumentType, object argument)
        {
            _args[argumentType] = argument;
        }

        public GridResults Data(GridRequest<T> input)
        {
            return buildGrid().Invoke(_services, input.ToDataRequest());
        }

        public int Count()
        {
            return buildGrid().Count(_services);
        }

        // TODO -- get a UT against this
        public DataTable ToDataTable(GridRequest<T> input)
        {
            var grid = buildGrid();

            var results = Data(input);
            var dataFields = new List<string>();
            results.items.Each(r =>
            {
                var dict = (IDictionary<string, string>) r.cell[0];
                dict.Keys.Each(k => dataFields.Fill(k));
            });

            var columnNames =
                grid.Definition.Columns.Skip(1).SelectMany(x => x.ToDictionary()).Select(x => (string) x["name"]);

            var table = new DataTable();
            dataFields.Each(x => table.Columns.Add(x, typeof (string)));
            columnNames.Each(x => table.Columns.Add(x, typeof (string)));

            results.items.Each(item =>
            {
                var list = new List<string>();
                var dict = (IDictionary<string, string>) item.cell[0];
                list.AddRange(dict.Values.Select(x => x.ToString()));
                list.AddRange(item.cell.Skip(1).Select(x => { return x == null ? string.Empty : x.ToString(); }));

                table.Rows.Add(list.Cast<object>().ToArray());
            });

            return table;
        }


        // TODO -- lots of unit tests here
        public JqGridModel BuildModel()
        {
            var grid = buildGrid();

            var gridName = typeof(T).NameForGrid();
            var definition = grid.Definition;
            var url = _urls.UrlFor(new GridRequest<T>{
                gridName = gridName
            });

            // TODO -- UT for this
            var entity = _args.OfType<DomainEntity>().SingleOrDefault();
            if (entity != null)
            {
                url += "?Id=" + entity.Id;
            }


            return new JqGridModel{
                colModel = definition.Columns.SelectMany(x => x.ToDictionary()).ToArray(),
                gridName = gridName,
                url = url,
                headers = definition.Columns.Select(x => x.GetHeader()).ToArray(),
                pagerId = gridName + "_pager",
            };
        }
    }
}