using FubuMVC.Core.Continuations;
using ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.HelloPartial;
using ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesPartial;

namespace ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesTransferTo
{
    public class TransferToEndpoint
    {
         public FubuContinuation Tranfer()
         {
             return FubuContinuation.TransferTo(new HelloPartialInputModel());
         }
    }
    public class RedirectToEndpoint
    {
         public FubuContinuation Redirect()
         {
             return FubuContinuation.RedirectTo<UsesPartialEndpoint>(x => x.Execute());
         }
    }
}