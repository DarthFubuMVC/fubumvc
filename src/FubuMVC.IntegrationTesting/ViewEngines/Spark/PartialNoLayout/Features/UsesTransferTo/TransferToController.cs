using FubuMVC.Core.Continuations;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.HelloPartial;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesTransferTo
{
    public class TransferToController
    {
         public FubuContinuation Tranfer()
         {
             return FubuContinuation.TransferTo(new HelloPartialInputModel());
         }
    }
}