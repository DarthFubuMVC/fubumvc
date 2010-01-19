using System;
using System.Collections.Generic;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public abstract class DescriptorFilterBase : IDescriptorFilter
    {
        #region IDescriptorFilter Members

        public abstract void ExtraParameters(ControllerContext context, IDictionary<string, object> extra);

        public abstract IEnumerable<string> PotentialLocations(IEnumerable<string> locations,
                                                               IDictionary<string, object> extra);

        #endregion

        protected static bool TryGetString(IDictionary<string, object> extra, string name, out string value)
        {
            object obj;
            if (extra.TryGetValue(name, out obj))
            {
                value = Convert.ToString(obj);
                return !string.IsNullOrEmpty(value);
            }

            value = null;
            return false;
        }
    }
}