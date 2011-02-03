using System.Collections.Generic;
using FubuFastPack.Querying;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public interface IGrid
    {
        IGridDefinition Definition { get; }

        // TODO -- let's move this inside.  Could use, *gasp* Setter injection here
        GridResults Invoke(IServiceLocator services, GridDataRequest request);

        IEnumerable<Criteria> BaselineCriterion { get; }
    }
}