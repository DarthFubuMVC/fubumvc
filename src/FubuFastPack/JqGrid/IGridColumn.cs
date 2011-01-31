using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{



    public interface IGridColumn
    {
        GridColumnDTO ToDto();
        Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls);
    }


}