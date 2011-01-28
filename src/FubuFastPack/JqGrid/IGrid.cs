using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public interface IGrid
    {
        IEnumerable<IGridColumn> Columns { get; }
        IGridDataSource BuildSource(IServiceLocator services);
    }
}