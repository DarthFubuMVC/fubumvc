using System;
using System.Collections;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using FubuFastPack.Querying;
using System.Linq;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    // Need one for projections, another for entities
    public interface IGridData
    {
        Func<object> GetterFor(Accessor accessor);
        bool MoveNext();
    }



    // One for projections, one for an entity
    public interface IGridDataSource
    {
        int TotalCount();
        IGridData Fetch(PagingOptions options);
    }

    public interface IGrid
    {
        IEnumerable<IGridColumn> Columns { get; }
        IGridDataSource BuildSource(IServiceLocator services);
    }

    public interface IGridColumn
    {
        GridColumnDTO ToDto();
        Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls);
    }

    

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



    public class EntityDTO
    {
        private readonly List<string> _cells = new List<string>();
        private readonly Cache<string, string> _properties = new Cache<string, string>();

        public string this[string key]
        {
            get
            {
                return _properties[key];
            }
            set
            {
                _properties[key] = value;
            }
        }

        public void AddCellDisplay(string display)
        {
            _cells.Add(display);
        }

        public object[] cell
        {
            get
            {
                var list = new List<object>();
                list.Add(_properties.ToDictionary());
                list.AddRange(_cells);

                return list.ToArray();
            }
            set
            {
                
            }
        }
    }
}