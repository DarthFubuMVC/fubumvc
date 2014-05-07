using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Tests.Docs.Topics.Authorization
{
  // SAMPLE: authorization-convention
  public class PurchasedProductAuthorizationPolicy : IConfigurationAction
  {
      public void Configure(BehaviorGraph graph)
      {
          graph.Actions()
              .Where(x => x.Method.Name.EndsWith("Review"))
              .Each(x => x.ParentChain().Authorization.AddPolicy(new PurchasedProductPolicy()));
      }
  }
  // ENDSAMPLE
}