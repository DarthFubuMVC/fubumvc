using FubuCore.Util;

namespace FubuMVC.Spark
{
    public interface IVisitor<in T>
    {
        void Visit(T instance);
    }

    public interface IVisitable<out T>
    {
        void AcceptVisitor(IVisitor<T> visitor);
    }

    public class BasicVisitor<T> : IVisitor<T>
    {
        public CompositeFilter<T> Filter { get; set; }
        public CompositeAction<T> Action { get; set; }

        public BasicVisitor()
        {
            Filter = new CompositeFilter<T>();
            Action = new CompositeAction<T>();
        }

        public void Visit(T instance)
        {
            if (Filter.MatchesAll(instance))
            {
                Action.Do(instance);
            }
        }
    }


}