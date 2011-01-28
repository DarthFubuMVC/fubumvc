using System;
using System.Collections;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using System.Linq;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    // Need one for projections, another for entities


    // One for projections, one for an entity


    public class GridColumn : IGridColumn
    {
        private readonly Accessor _accessor;

        public GridColumn(Accessor accessor)
        {
            _accessor = accessor;
        }

        public GridColumnDTO ToDto()
        {
            throw new NotImplementedException();
        }

        // TODO -- UT this
        public Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var source = data.GetterFor(_accessor);
            return dto =>
            {
                var rawValue = source();

                var display = formatter.GetDisplayForValue(_accessor, rawValue);
                dto.AddCellDisplay(display);
            };
        }
    }
}