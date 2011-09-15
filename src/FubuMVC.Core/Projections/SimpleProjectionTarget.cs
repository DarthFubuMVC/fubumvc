using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Projections
{
    public class SimpleProjectionTarget : IProjectionTarget
    {
        private readonly object _subject;
        private readonly IUrlRegistry _urls;

        public SimpleProjectionTarget(object subject) : this(subject, new StubUrlRegistry())
        {
        }

        public SimpleProjectionTarget(object subject, IUrlRegistry urls)
        {
            _subject = subject;
            _urls = urls;
        }

        public object ValueFor(Accessor accessor)
        {
            return accessor.GetValue(_subject);
        }

        public IUrlRegistry Urls
        {
            get { return _urls; }
        }

        public object Subject
        {
            get { return _subject; }
        }
    }
}