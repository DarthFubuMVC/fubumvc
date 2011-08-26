using FubuMVC.Core;
using FubuMVC.GuideApp.Behaviors;
using FubuMVC.GuideApp.Controllers.Home;

namespace FubuMVC.GuideApp
{
    public class AdvBehaviorGuideRegistry : FubuRegistry
    {
        public AdvBehaviorGuideRegistry()
        {
            IncludeDiagnostics(true);

            Applies.ToThisAssembly();

            Actions
                .IncludeTypesNamed(x => x.EndsWith("Controller"));

            Routes
                .IgnoreControllerNamespaceEntirely();

            Views
                .TryToAttach(x =>
                {
                    x.by_ViewModel_and_Namespace_and_MethodName();
                    x.by_ViewModel_and_Namespace();
                    x.by_ViewModel();
                });

            Policies
                .EnrichCallsWith<DemoBehaviorForSelectActions>(
                    call => call.Method.Name == "Home"
                )
                .EnrichCallsWith<DemoBehaviorForSelectActions>(
                    call => call.Returns<HomeViewModel>()
                );

            Policies
                .Add<DemoBehaviorPolicy>();
                

            Routes.HomeIs<HomeInputModel>();
        }
    }
}