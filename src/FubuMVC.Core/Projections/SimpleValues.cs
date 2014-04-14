using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    public class SimpleValues<T> : IValues<T>
    {
        private readonly T _subject;

        public SimpleValues(T subject)
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