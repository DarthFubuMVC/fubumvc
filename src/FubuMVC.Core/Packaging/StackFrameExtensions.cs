using System.Diagnostics;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public static class StackFrameExtensions
    {
        public static string ToDescription(this StackFrame frame)
        {
            return "{0}.{1}(), {2} line {3}".ToFormat(frame.GetMethod().DeclaringType.FullName, frame.GetMethod().Name,
                                                      frame.GetFileName(), frame.GetFileLineNumber());
        }
    }
}