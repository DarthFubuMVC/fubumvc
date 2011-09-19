using System;
using System.Linq;

namespace FubuMVC.Core.Http
{
    public class MimetypeRequestConditional : IRequestConditional
    {
        private readonly string[] _mimeTypes;

        public MimetypeRequestConditional(params string[] mimeTypes)
        {
            _mimeTypes = mimeTypes;
        }

        public bool Matches(string mimeType)
        {
            return _mimeTypes.Contains(mimeType);
        }
    }
}