using FubuCore.Reflection;

namespace FubuMVC.Core.Rest.Projections
{
    public class SimpleValueSource<T> : IValueSource<T>
    {
        private readonly T _subject;

        public SimpleValueSource(T subject)
        {
            _subject = subject;
        }

        public object ValueFor(Accessor accessor)
        {
            return accessor.GetValue(_subject);
        }

        public T Subject
        {
            get { return _subject; }
        }
    }
}