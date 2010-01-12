namespace FubuMVC.Core.Registration.ObjectGraph
{
    public interface IDependencyVisitor
    {
        void Value(ValueDependency dependency);
        void Configured(ConfiguredDependency dependency);
    }
}