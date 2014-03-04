using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.View
{
    public static class FubuRegistryViewExtensions
    {
         public static void ViewFacility(this FubuRegistry registry, IViewFacility facility)
         {
             registry.AlterSettings<ViewEngines>(x => x.AddFacility(facility));

             registry.Policies.Add<ViewAttacher>();
             registry.Policies.Add<ActionlessViewConvention>();
             registry.Policies.Add<AutoImportModelNamespacesConvention>();
         }
    }
}