using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuCore;

namespace FubuFastPack.JqGrid
{
    public class EntityDTO
    {
        private readonly List<object> _cells = new List<object>();
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

        public bool HasProperty(string key)
        {
            return _properties.Has(key);
        }

        public void AddCellDisplay(string display)
        {
            _cells.Add(display.HtmlEncode());
        }

        public object[] cell
        {
            get
            {
                var list = new List<object>{
                    _properties.ToDictionary()
                };
                list.AddRange(_cells);

                return list.ToArray();
            }
            set
            {
                
            }
        }

        public Guid Id()
        {
            return Guid.Parse(this["Id"]);
        }
    }
}