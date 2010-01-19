using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public class AreaDescriptorFilter : DescriptorFilterBase
    {
        public override void ExtraParameters(ControllerContext context, IDictionary<string, object> extra)
        {
            object value;
            if (context.RouteData.Values.TryGetValue("area", out value))
                extra["area"] = value;
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
