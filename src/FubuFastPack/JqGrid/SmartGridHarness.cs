using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuLocalization;
using FubuMVC.Core;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    // TODO -- only build the grid once
    public class SmartGridHarness<T> : ISmartGridHarness where T : ISmartGrid
    {
        private readonly Cache<string, object> _args = new Cache<string, object>();
        private readonly IQueryService _queryService;
        private readonly ISmartRequest _request;
        private readonly IEnumerable<IGridPolicy> _globalPolicies;
        private readonly IServiceLocator _services;
        private readonly IEndpointService _endpointService;

        public SmartGridHarness(IServiceLocator services, IEndpointService endpointService, IQueryService queryService, ISmartRequest request, IEnumerable<IGridPolicy> globalPolicies)
        {
            _services = services;
            _endpointService = endpointService;
            _queryService = queryService;
            _request = request;
            _globalPolicies = globalPolicies;
        }

        public Type GridType
        {
            get { return typeof (T); }
        }

        // TODO -- hit this with StoryTeller
        public IEnumerable<FilteredProperty> FilteredProperties(T grid)
        {
            return grid.AllFilteredProperties(_queryService);
        }

        public T BuildGrid()
        {
            var args = buildArgs();

            var grid = (T) Activator.CreateInstance(typeof (T), args);
            grid.ApplyPolicies(_globalPolicies);

            return grid;
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

            if (parameter.ParameterType.CanBeCastTo<DomainEntity>())
            {
                if (value != null && !(value is DomainEntity))
                {
                    var message = "Type {0} received for parameter {1}, but expected {2}"
                        .ToFormat(value.GetType().FullName, name, parameter.ParameterType.FullName);
                    throw new SmartGridException(message);
                }
            }
            else
            {
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
            }




            _args[name] = value;
        }

        private object[] buildArgs()
        {
            var ctor = getConstructor();
            return ctor.GetParameters().Select(p =>
            {
                if (_args.Has(p.Name))
                {
                    return _args[p.Name];
                }

                var arg = _request.Value(p.ParameterType, p.Name);
                if (arg == null && !p.ParameterType.CanBeCastTo<DomainEntity>())
                {
                    throw new SmartGridException("Querystring argument {0} cannot be found".ToFormat(p.Name));
                }

                return arg;
            }).ToArray();
        }

        private ConstructorInfo getConstructor()
        {
            return typeof (T).GetConstructors().Single();
        }

        public string GetUrl()
        {
            var url = _endpointService.EndpointFor(new GridRequest<T>()).Url;

            url += GetQuerystring();

            return url;
        }

        public string GetQuerystring()
        {
            var ctor = getConstructor();
            if (ctor.GetParameters().Any())
            {
                var queryStrings = ctor.GetParameters().Select(p => buildQueryStringForArg(p.ParameterType, p.Name));

                return "?" + queryStrings.Join("&");
            }

            return string.Empty;
        }

        private string buildQueryStringForArg(Type type, string key)
        {
            var value = _args.Has(key) ? _args[key] : _request.Value(type, key);
            var stringValue = type.CanBeCastTo<DomainEntity>()
                ? (value == null ? string.Empty : value.As<DomainEntity>().Id.ToString())
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

        public int Count<TEntity>(IDataRestriction<TEntity> restriction) where TEntity : DomainEntity
        {
            return BuildGrid().As<ISmartGrid<TEntity>>().Count(_services, restriction);
        }

        public Type EntityType()
        {
            return BuildGrid().GetType().FindInterfaceThatCloses(typeof (ISmartGrid<>)).GetGenericArguments().First();
        }


        // TODO -- get an E to E test on this mess
        public Guid IdOfFirstResult()
        {
            var request = new GridRequest<T>(){
                page = 1,
                rows = 1
            };

            var dto = Data(request);
            return dto.items.First().Id();
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
                grid.Definition.Columns.Skip(1).SelectMany(x => x.ToDictionary()).Select(x => (string) x["name"]).ToList();



            var table = new DataTable();
            dataFields.Each(x => table.Columns.Add(x, typeof (string)));


            columnNames.Each(x =>
            {
                var name = x;
                while (table.Columns.IndexOf(name) > -1)
                {
                    name += "1";
                }
                table.Columns.Add(name, typeof (string));
            });

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
        public JqGridModel BuildJqModel()
        {
            var grid = BuildGrid();

            return buildJqModel(grid);
        }

        private JqGridModel buildJqModel(T grid)
        {
            var gridName = typeof (T).NameForGrid();
            var definition = grid.Definition;
            var sorting = definition.SortOrder();

            return new JqGridModel{
                colModel = definition.Columns.SelectMany(x => x.ToDictionary()).ToArray(),
                gridName = gridName,
                containerName = typeof(T).ContainerNameForGrid(),
                arguments = GetArgumentsAsString().Join(","),
                url = GetUrl(),
                headers = definition.Columns.SelectMany(x => x.Headers()).ToArray(),
                pagerId = gridName + "_pager",
                initialCriteria = grid.InitialCriteria().ToArray(),
                sortname = sorting.sortname,
                sortorder = sorting.sortorder
            };
        }

        public IEnumerable<string> GetArgumentsAsString()
        {
            foreach (var buildArg in buildArgs())
            {
                var entity = buildArg as DomainEntity;
                if (entity != null)
                {
                    yield return entity.Id.ToString();
                }
                else
                {
                    yield return buildArg == null ? string.Empty: buildArg.ToString();
                }

                
            }
        }



        public GridViewModel BuildGridModel(IEnumerable<IGridPolicy> gridPolicies)
        {
            var grid = BuildGrid();
            
            // TODO -- get an integration test against this
            grid.ApplyPolicies(gridPolicies ?? new IGridPolicy[0]);

            var model = new GridViewModel(){
                AllowCreateNew = grid.Definition.AllowCreationOfNew,
                CanSaveQuery = grid.Definition.CanSaveQuery,
                FilteredProperties = FilteredProperties(grid),
                GridModel = buildJqModel(grid),
                GridName = typeof(T).NameForGrid(),
                GridType = typeof(T),
                HeaderText = grid.GetHeader()
            };

            model.AddCriterion(grid.InitialCriteria());
            
            setupCreateNewProperties(grid, model);

            return model;
        }

        private void setupCreateNewProperties(T grid, GridViewModel model)
        {
            if (!grid.Definition.AllowCreationOfNew) return;

            var endpoint = _endpointService.EndpointForNew(grid.EntityType);

            if (!endpoint.IsAuthorized)
            {
                model.AllowCreateNew = false;
                return;
            }
            
            model.NewEntityText = StringToken.FromKeyString("CREATE_NEW_" + grid.EntityType.Name.ToUpper()).ToString();
            model.NewEntityUrl = endpoint.Url;
        }

        public void RegisterArguments(params object[] arguments)
        {
            var ctor = getConstructor();

            var parameters = ctor.GetParameters();
            for (int i = 0; i < arguments.Length; i++) // Important to do this to the argument length
            {
                var parameter = parameters[i];
                var arg = arguments[i];

                if (arg != null) RegisterArgument(parameter.Name, arg);
            }
        }

        public string HeaderText()
        {
            return BuildGrid().GetHeader();
        }
    }
}