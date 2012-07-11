using System.Collections.Generic;
using FubuMVC.Core.Urls;

namespace FubuMVC.SlickGrid
{
    public interface IGridDefinition<T> : IGridDefinition
    {
        IEnumerable<IDictionary<string, object>> FormatData(IEnumerable<T> data);
    }

    public interface IGridDefinition
    {
        string ToColumnJson();
        string SelectDataSourceUrl(IUrlRegistry urls);
    }
}