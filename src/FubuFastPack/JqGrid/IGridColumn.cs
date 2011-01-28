using System;
using FubuCore;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public interface IGridColumn
    {
        GridColumnDTO ToDto();
        Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls);
    }
}