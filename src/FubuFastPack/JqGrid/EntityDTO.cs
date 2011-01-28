using System.Collections.Generic;
using FubuCore.Util;

namespace FubuFastPack.JqGrid
{
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