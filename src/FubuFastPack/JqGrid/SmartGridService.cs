using System;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

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
        public string GridName { get; set; }
    }

    public interface ISmartGridService
    {
        GridCounts GetCounts<T>(params object[] args) where T : ISmartGrid;
        GridCounts GetCounts(string gridName, params object[] args);
        GridViewModel GetModel(NamedGridRequest request);
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

        public GridCounts GetCounts<T>(params object[] args) where T : ISmartGrid
        {
            var harness = _locator.GetInstance<SmartGridHarness<T>>();
            return getCounts(harness, args);
        }

        private GridCounts getCounts(ISmartGridHarness harness, object[] args)
        {
            harness.RegisterArguments(args);

            return new GridCounts(){
                Count = harness.Count(),
                Url = _urls.UrlFor(new NamedGridRequest(){GridName = harness.GridType.NameForGrid()}) + harness.GetQuerystring(),
                HeaderText = harness.HeaderText()
            };
        }

        public GridCounts GetCounts(string gridName, params object[] args)
        {
            var harness = _locator.GetInstance<ISmartGridHarness>(gridName);
            return getCounts(harness, args);
        }

        public GridViewModel GetModel(NamedGridRequest request)
        {
            var harness = _locator.GetInstance<ISmartGridHarness>(request.GridName);
            return harness.BuildGridModel();
        }
    }


}