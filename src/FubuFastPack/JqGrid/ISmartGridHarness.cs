using System;
using System.Collections.Generic;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public interface ISmartGridHarness
    {
        Type GridType { get; }
        IEnumerable<FilteredProperty> FilteredProperties();
    }
}