using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.UI.Security
{
    public static class AccessRightExtensions
    {
        public static AccessRight Most(this IEnumerable<AccessRight> rights)
        {
            return AccessRight.Most(rights.ToArray());
        }

        public static AccessRight Least(this IEnumerable<AccessRight> rights)
        {
            return AccessRight.Least(rights.ToArray());
        }
    }
}