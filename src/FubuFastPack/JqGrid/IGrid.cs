using FubuFastPack.Querying;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public interface IGrid
    {
        IGridDefinition Definition { get; }
        GridResults Invoke(IServiceLocator services, PagingOptions request);
    }
}