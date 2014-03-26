using System;
using FubuCore.Binding;

namespace FubuMVC.Diagnostics.Runtime
{
    public class DebugDetector : IDebugDetector
    {
        public static readonly string FLAG = "FubuDebug";
        private readonly Lazy<bool> _test;

        public DebugDetector(IRequestData request)
        {
            _test = new Lazy<bool>(() =>
            {
                bool returnValue = false;
                request.Value(FLAG, o => returnValue = true);

                return returnValue;
            });
        }

        public bool IsDebugCall()
        {
            return _test.Value;
        }
    }
}