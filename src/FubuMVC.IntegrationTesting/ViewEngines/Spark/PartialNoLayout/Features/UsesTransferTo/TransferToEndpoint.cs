using FubuMVC.Core.Continuations;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.HelloPartial;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesPartial;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesTransferTo
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