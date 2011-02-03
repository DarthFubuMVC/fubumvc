using System.Collections.Generic;
using FubuCore.Reflection;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    
    public interface IGridDefinition
    {
        IEnumerable<IGridColumn> Columns { get; }
        IEnumerable<Accessor> SelectedAccessors { get; }
        IEnumerable<FilterDTO> AllPossibleFilters(IQueryService queryService); 
    }
}