using System;
using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics
{
    public class DebugDetector : IDebugDetector
    {
        public static readonly string FLAG = "FubuDebug";
        private readonly Lazy<bool> _test;
        private bool _latched = true;

        public DebugDetector(IRequestData request)
        {
            _test = new Lazy<bool>(() =>
            {
                bool returnValue = false;
                request.Value(FLAG, o => returnValue = true);

                return returnValue;
            });
        }

        public virtual bool IsOutputWritingLatched()
        {
            return IsDebugCall() && _latched;
        }

        public void UnlatchWriting()
        {
            _latched = false;
        }

        public bool IsDebugCall()
        {
            return _test.Value;
        }
    }
}