using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Etags
{
    public interface IEtagCache
    {
        // Can be null
        string CurrentETag(string resourcePath);
        void WriteCurrentETag(string resourcePath, string etag);
    }

    public interface IETagGenerator<T>
    {
        string Create(T target);
    }

    public class ETagBehavior<T> : BasicBehavior
    {
        private readonly IOutputWriter _writer;
        private readonly IEtagCache _cache;
        private readonly ISmartRequest _request;
        private readonly IETagGenerator<T> _generator;

        public ETagBehavior(IOutputWriter writer, IEtagCache cache, ISmartRequest request, IETagGenerator<T> generator) : base(PartialBehavior.Ignored)
        {
            _writer = writer;
            _cache = cache;
            _request = request;
            _generator = generator;
        }

        protected override DoNext performInvoke()
        {
            throw new NotImplementedException();
        }

        protected override void afterInsideBehavior()
        {
            throw new NotImplementedException();
        }
    }
}