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
using FubuMVC.Core;
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


    public class EntityGridData<T> : IGridData
    {
        private T _current;
        private Queue<T> _queue;

        public EntityGridData(IEnumerable<T> entities)
        {
            _queue = new Queue<T>(entities);
        }

        public Func<object> GetterFor(Accessor accessor)
        {
            return () => accessor.GetValue(_current);
        }

        public bool MoveNext()
        {
            if (_queue.Any())
            {
                _current = _queue.Dequeue();
                return true;
            }

            return false;
        }
    }

    // One for projections, one for an entity
    public interface IGridDataSource
    {
        int TotalCount();
        IGridData Fetch(PagingOptions options);
    }

    public class GridRunner
    {
        private readonly IDisplayFormatter _formatter;
        private readonly IUrlRegistry _urls;
        private readonly IServiceLocator _services;

        public GridRunner(IDisplayFormatter formatter, IUrlRegistry urls, IServiceLocator services)
        {
            _formatter = formatter;
            _urls = urls;
            _services = services;
        }

        public GridResults Fetch(PagingOptions paging, IGrid grid)
        {
            var source = grid.BuildSource(_services);
            var data = source.Fetch(paging);
            var actions = grid.Columns.Select(col => col.CreateFiller(data, _formatter, _urls));

            var list = new List<EntityDTO>();

            while (data.MoveNext())
            {
                var dto = new EntityDTO();
                actions.Each(a => a(dto));

                list.Add(dto);
            }

            // TODO -- needs some UT's
            int recordCount = source.TotalCount();
            var pageCount = (int)Math.Ceiling(recordCount / (double)paging.ResultsPerPage);

            if (pageCount < paging.Page)
            {
                paging.Page = pageCount;
            }

            return new GridResults(){
                page = paging.Page,
                records = recordCount,
                total = pageCount,
                items = list
            };
        }


    }

    public class GridResults : JsonMessage
    {
        private static readonly GridResults _empty = new GridResults
        {
            items = new EntityDTO[0]
        };

        public int total { get; set; }
        public int lastpage { get { return total; } set { } }
        public bool viewrecords { get { return true; } set { } }
        public int page { get; set; }
        public int records { get; set; }
        public IEnumerable<EntityDTO> items { get; set; }
        public string Description { get; set; }

        public static GridResults Empty { get { return _empty; } }

        public override string ToString()
        {
            return string.Format("total: {0}, page: {1}, records: {2}, items: {3}, Description: {4}", total, page, records, items.Count(), Description);
        }
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

    // Keep this as a stupid data bag
    public class GridColumnDTO
    {
        public const string DataColumnFormatterType = "data";

        public GridColumnDTO(string name, string header)
        {
            Name = name;
            Header = header;
            Sortable = true;
            DisplayType = DataColumnFormatterType;
        }

        public string DisplayType { get; set; }
        public string Name { get; private set; }
        public string Header { get; private set; }
        public bool Sortable { get; set; }
        public string EditType { get; set; }

        public bool Equals(GridColumnDTO obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Name, Name) && Equals(obj.Header, Header) && obj.Sortable.Equals(Sortable) && Equals(obj.EditType, EditType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(GridColumnDTO)) return false;
            return Equals((GridColumnDTO)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result * 397) ^ (Header != null ? Header.GetHashCode() : 0);
                result = (result * 397) ^ Sortable.GetHashCode();
                result = (result * 397) ^ (EditType != null ? EditType.GetHashCode() : 0);
                return result;
            }
        }
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