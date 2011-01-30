using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public interface IGrid
    {
        GridDefinition Definition { get; }
        IGridDataSource BuildSource(IServiceLocator services);
    }
}