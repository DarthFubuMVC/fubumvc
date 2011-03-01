using System;
using System.Collections.Generic;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using FubuFastPack.JqGrid;

namespace FubuFastPack.JqGrid
{
    public class GridState
    {
        public string GridId { get; set; }
        public string Url { get; set; }
        public int Count { get; set; }
        public string HeaderText { get; set; }
        public string ContainerId { get; set; }
        public string LabelId { get; set; }
    }


    public class GridCounts
    {
        public string HeaderText { get; set; }
        public int Count { get; set; }
        public string Url { get; set; }
    }

    public class NamedGridRequest
    {
        public static NamedGridRequest For<T>() where T : ISmartGrid
        {
            return new NamedGridRequest(){
                GridName = typeof(T).NameForGrid()
            };
        }

        public string GridName { get; set; }
        public IEnumerable<IGridPolicy> Policies { get; set; }
        public IEnumerable<object> Arguments { get; set; }

        public bool Equals(NamedGridRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.GridName, GridName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (NamedGridRequest)) return false;
            return Equals((NamedGridRequest) obj);
        }

        public override int GetHashCode()
        {
            return (GridName != null ? GridName.GetHashCode() : 0);
        }
    }

    public interface IGridPolicy
    {
        void AlterDefinition<T>(GridDefinition<T> definition) where T : DomainEntity;
        void AlterGrid(ISmartGrid grid);
    }

    public interface ISmartGridService
    {
        GridCounts GetCounts<TGrid, TInput>(params object[] args)
            where TGrid : ISmartGrid
            where TInput : NamedGridRequest, new();

        GridCounts GetCounts<TInput>(string gridName, params object[] args) where TInput : NamedGridRequest, new();
        GridViewModel GetModel(NamedGridRequest request);
        string QuerystringFor(string gridName, params object[] args);

        Type EntityTypeForGrid(string gridName);

        GridState StateForGrid<TGrid>(params object[] args) where TGrid : ISmartGrid;

        int RecordCountFor<TGrid, TEntity>(IDataRestriction<TEntity> restriction, params object[] arguments) 
            where TEntity : DomainEntity
            where TGrid : ISmartGrid<TEntity>;

        int RecordCountFor<TGrid>(params object[] arguments) where TGrid : ISmartGrid;

        string GetUrl<TInput, TGrid>(params object[] args) where TInput : NamedGridRequest, new() where TGrid : ISmartGrid;

        Guid IdOfFirstResult<TGrid>(params object[] args) where TGrid : ISmartGrid;
    }

    public class SmartGridService : ISmartGridService
    {
        private readonly IServiceLocator _locator;
        private readonly IUrlRegistry _urls;
        
        // TODO -- replace with "typed factories"?
        public SmartGridService(IServiceLocator locator, IUrlRegistry urls)
        {
            _locator = locator;
            _urls = urls;
        }

        public GridCounts GetCounts<TGrid, TInput>(params object[] args) where TGrid : ISmartGrid where TInput : NamedGridRequest, new()
        {
            var harness = _locator.GetInstance<SmartGridHarness<TGrid>>();
            return getCounts<TInput>(harness, args);
        }

        private GridCounts getCounts<TInput>(ISmartGridHarness harness, object[] args) where TInput : NamedGridRequest, new()
        {
            harness.RegisterArguments(args);

            return new GridCounts(){
                Count = harness.Count(),
                Url = _urls.UrlFor(new TInput(){GridName = harness.GridType.NameForGrid()}) + harness.GetQuerystring(),
                HeaderText = harness.HeaderText()
            };
        }

        public GridCounts GetCounts<TInput>(string gridName, params object[] args) where TInput : NamedGridRequest, new()
        {
            var harness = _locator.GetInstance<ISmartGridHarness>(gridName);
            return getCounts<TInput>(harness, args);
        }

        public GridViewModel GetModel(NamedGridRequest request)
        {
            var harness = _locator.GetInstance<ISmartGridHarness>(request.GridName);
            if (request.Arguments != null)
            {
                harness.RegisterArguments(request.Arguments.ToArray());
            }
            
            return harness.BuildGridModel(request.Policies ?? new IGridPolicy[0]);
        }

        public string QuerystringFor(string gridName, params object[] args)
        {
            var harness = getHarnessByName(gridName, args);

            return harness.GetQuerystring();
        }

        public Type EntityTypeForGrid(string gridName)
        {
            var harness = getHarnessByName(gridName);
            return harness.EntityType();
        }

        public GridState StateForGrid<TGrid>(params object[] args) where TGrid : ISmartGrid
        {
            var gridType = typeof(TGrid);
            var harness = getHarness<TGrid>(args);
            
            return new GridState(){
                ContainerId = gridType.NameForGrid() + "-container",
                GridId = gridType.ContainerNameForGrid(),
                Count = harness.Count(),
                HeaderText = harness.HeaderText(),
                LabelId = gridType.IdForLabel(),
                Url = _urls.UrlFor(new GridRequest<TGrid>()) + harness.GetQuerystring()
            };
        }

        // TODO -- eliminate some of the duplication
        // More testing.
        // This is well tested in Dovetail code, but want tests in fastpack code where it belongs
        public int RecordCountFor<TGrid, TEntity>(IDataRestriction<TEntity> restriction, params object[] arguments) where TGrid : ISmartGrid<TEntity> where TEntity : DomainEntity
        {
            var harness = getHarness<TGrid>(arguments);
            return harness.Count(restriction);
        }

        public int RecordCountFor<TGrid>(params object[] arguments) where TGrid : ISmartGrid
        {
            return getHarness<TGrid>(arguments).Count();
        }

        private ISmartGridHarness getHarness<TGrid>(object[] args)
        {
            var gridName = typeof(TGrid).NameForGrid();
            return getHarnessByName(gridName, args);
        }

        private ISmartGridHarness getHarnessByName(string gridName, params object[] args)
        {
            var harness = _locator.GetInstance<ISmartGridHarness>(gridName);
            harness.RegisterArguments(args);
            return harness;
        }

        public string GetUrl<TInput, TGrid>(params object[] args) where TInput : NamedGridRequest, new() where TGrid : ISmartGrid
        {
            var harness = getHarness<TGrid>(args);
            var url = _urls.UrlFor(new TInput{
                GridName = typeof (TGrid).NameForGrid()
            });

            url += harness.GetQuerystring();

            return url;
        }

        // TODO -- get and end to end test
        public Guid IdOfFirstResult<TGrid>(params object[] args) where TGrid : ISmartGrid
        {
            var harness = getHarness<TGrid>(args);
            return harness.IdOfFirstResult();
        }
    }


}