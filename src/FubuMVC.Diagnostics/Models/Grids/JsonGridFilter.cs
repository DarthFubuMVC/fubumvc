using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Diagnostics.Models.Grids
{
    public class JsonGridFilter
    {
        public string ColumnName { get; set; }
        public IEnumerable<string> Values { get; set; }

        public bool Matches(string value, Func<string, string, bool> comparison)
        {
            return Values.Any(v => comparison(v.ToLower(), value.ToLower()));
        }
    }
}