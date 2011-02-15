using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FubuCore.Binding;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;
using FubuCore;

namespace FubuFastPack.JqGrid
{
    public class SmartGridException : Exception
    {
        public SmartGridException(string message) : base(message)
        {
        }

        public SmartGridException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SmartGridException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


    // Right now, SmartGrid's can *only* have a dependency on 1 Entity in the ctor
    public class SmartGridHarness<T> where T : ISmartGrid
    {
        private readonly IQueryService _queryService;
        private readonly ISmartRequest _request;
        private readonly IServiceLocator _services;
        private readonly IUrlRegistry _urls;
        private readonly Cache<string, object> _args = new Cache<string, object>();

        public SmartGridHarness(IServiceLocator services, IUrlRegistry urls, IQueryService queryService, ISmartRequest request)
        {
            _services = services;
            _urls = urls;
            _queryService = queryService;
            _request = request;
        }

        public T BuildGrid()
        {
            var args = buildArgs();

            return (T) Activator.CreateInstance(typeof (T), args);
        }

        

        public void RegisterArgument(string name, object value)
        {
            var ctor = getConstructor();
            var parameter = ctor.GetParameters().FirstOrDefault(x => x.Name == name);
            
            if (parameter == null)
            {
                var argList = ctor.GetParameters().Select(x => x.Name).Join(", ");
                var message = "Argument {0} is invalid.  The possible arguments are {1}"
                    .ToFormat(name, argList);

                throw new SmartGridException(message);
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (!value.GetType().CanBeCastTo(parameter.ParameterType))
            {
                var message = "Type {0} received for parameter {1}, but expected {2}"
                    .ToFormat(value.GetType().FullName, name, parameter.ParameterType.FullName);
                throw new SmartGridException(message);
            }


            _args[name] = value;
        }

        // TODO -- deal with nulls and missing arguments
        private object[] buildArgs()
        {
            ConstructorInfo ctor = getConstructor();
            return ctor.GetParameters().Select(p =>
            {
                return _args.Has(p.Name) ? _args[p.Name] : _request.Value(p.ParameterType, p.Name);
            }).ToArray();
        }

        private ConstructorInfo getConstructor()
        {
            return typeof(T).GetConstructors().Single();
        }

        public string GetUrl()
        {
            var url = _urls.UrlFor(new GridRequest<T>());

            var ctor = getConstructor();
            if (ctor.GetParameters().Any())
            {
                var queryStrings = ctor.GetParameters().Select(p => buildQueryStringForArg(p.ParameterType, p.Name));
                url += "?" + queryStrings.Join("&");
            }

            return url;
        }

        private string buildQueryStringForArg(Type type, string key)
        {
            var value = _args.Has(key) ? _args[key] : _request.Value(type, key);
            var stringValue = type.CanBeCastTo<DomainEntity>()
                                  ? value.As<DomainEntity>().Id.ToString()
                                  : value.ToString().UrlEncoded();

            return "{0}={1}".ToFormat(key, stringValue);
        }

        public GridResults Data(GridRequest<T> input)
        {
            return BuildGrid().Invoke(_services, input.ToDataRequest());
        }

        public int Count()
        {
            return BuildGrid().Count(_services);
        }

        // TODO -- get a UT against this
        public DataTable ToDataTable(GridRequest<T> input)
        {
            var grid = BuildGrid();

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
            var grid = BuildGrid();

            var gridName = typeof(T).NameForGrid();
            var definition = grid.Definition;

            // TODO -- UT for this
            //var entity = _args.OfType<DomainEntity>().SingleOrDefault();
            //if (entity != null)
            //{
            //    url += "?Id=" + entity.Id;
            //}
            

            return new JqGridModel{
                colModel = definition.Columns.SelectMany(x => x.ToDictionary()).ToArray(),
                gridName = gridName,
                url = GetUrl(),
                headers = definition.Columns.Select(x => x.GetHeader()).ToArray(),
                pagerId = gridName + "_pager",
            };
        }
    }
}