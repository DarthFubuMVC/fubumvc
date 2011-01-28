using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;

namespace FubuFastPack.JqGrid
{
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
}