using System;
using FubuCore.Reflection;

namespace FubuFastPack.JqGrid
{
    public interface IGridData
    {
        Func<object> GetterFor(Accessor accessor);
        bool MoveNext();

        object CurrentRowType();
    }
}