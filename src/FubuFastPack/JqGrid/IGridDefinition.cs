using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuFastPack.JqGrid
{
    public interface IGridDefinition
    {
        IEnumerable<IGridColumn> Columns { get; }
        IEnumerable<Accessor> SelectedAccessors { get; }
    }
}