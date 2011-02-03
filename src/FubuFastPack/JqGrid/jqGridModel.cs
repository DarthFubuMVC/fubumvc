using System.Collections.Generic;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public class JqGridModel
    {
        public IDictionary<string, object>[] colModel { get; set; }
        public Criteria[] baselineCriterion { get; set; }
        public FilterDTO[] filters { get; set; }
        public string gridName { get; set; }
        public string url { get; set; }
    }
}