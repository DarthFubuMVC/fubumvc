using FubuMVC.Core;
using FubuMVC.StructureMap.Bootstrap;
using FubuMVC.View.Spark;

namespace FubuMVC.HelloWorld
{
    public class Global : FubuStructureMapApplication
    {
        public override FubuRegistry GetMyRegistry()
        {
            return new HelloWorldFubuRegistry();
        }
    }

    public class HelloWorldFubuRegistry : FubuRegistry
    {
        public HelloWorldFubuRegistry()
        {
            IncludeDiagnostics(true);

            Applies.ToThisAssembly();

            Actions
                .IncludeTypesNamed(x => x.EndsWith("Controller"));

            Routes
                .IgnoreControllerNamespaceEntirely();

            Views
                .TryToAttach(x=>
                {
                    x.to_spark_view_by_action_namespace_and_name(GetType().Namespace);
                    x.by_ViewModel_and_Namespace_and_MethodName();
                    x.by_ViewModel_and_Namespace();
                    x.by_ViewModel();
                });

            
        }
    }
}