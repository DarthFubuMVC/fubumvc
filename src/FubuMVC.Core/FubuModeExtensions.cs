using System;
using FubuCore;

namespace FubuMVC.Core
{

    /// <summary>
    /// Detects the machine or environment "mode" of a FubuMVC application.  Generally used
    /// to latch development features
    /// </summary>
    public static class FubuModeExtensions
    {
        public static readonly string Development = "Development";
        public static readonly string Testing = "Testing";


        /// <summary>
        /// Is the "Development" mode detected?
        /// </summary>
        /// <returns></returns>
        public static bool InDevelopment(this string mode)
        {
            if (mode.IsEmpty()) return false;

            return mode.Equals(Development, StringComparison.OrdinalIgnoreCase);
        }

        public static bool InDiagnostics(this string mode)
        {
            if (mode.IsEmpty()) return false;

            return mode.Equals("diagnostics", StringComparison.OrdinalIgnoreCase);
        }

        public static bool InTesting(this string mode)
        {
            if (mode.IsEmpty()) return false;

            return mode.Equals("testing", StringComparison.OrdinalIgnoreCase);
        }

    }


}