using System.Collections.Generic;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public class JqGridModel
    {
        public string arguments { get; set;}
        public IDictionary<string, object>[] colModel { get; set; }
        public string gridName { get; set; }
        public string containerName { get; set; }
        public string url { get; set; }
        public string[] headers { get; set; }
        public string pagerId { get; set; }
        public Criteria[] initialCriteria { get; set; }

        public string sortname { get; set; }
        public string sortorder { get; set; }
    }
}