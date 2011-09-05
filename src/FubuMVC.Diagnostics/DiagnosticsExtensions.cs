using System;
using FubuMVC.Core.Diagnostics;

namespace FubuMVC.Diagnostics
{
    public static class DiagnosticsExtensions
    {
        public static bool IsDiagnosticsReport(this Type type)
        {
            return typeof (IBehaviorDetails).IsAssignableFrom(type) ||
                   typeof (IModelBindingDetail).IsAssignableFrom(type);
        }
    }
}