using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public class AreaDescriptorFilter : DescriptorFilterBase
    {
        public override void ExtraParameters(ActionContext context, IDictionary<string, object> extra)
        {
            object value;
            if (context.Params.TryGetValue("area", out value))
            {
                extra["area"] = value;
            }
        }

        public override IEnumerable<string> PotentialLocations(IEnumerable<string> locations, IDictionary<string, object> extra)
        {
            string areaName;

            return TryGetString(extra, "area", out areaName)
                       ? locations.Select(x => Path.Combine(areaName, x)).Concat(locations)
                       : locations;
        }
    }
}