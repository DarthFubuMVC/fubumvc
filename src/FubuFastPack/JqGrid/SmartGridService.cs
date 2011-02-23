using System;
using System.Collections.Generic;
using FubuFastPack.Domain;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace FubuFastPack.JqGrid
{
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
            var harness = _locator.GetInstance<ISmartGridHarness>(gridName);
            harness.RegisterArguments(args);

            return harness.GetQuerystring();
        }
    }


}