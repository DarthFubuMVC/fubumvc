using FubuMVC.Core;

namespace FubuTransportation.Web
{

    public class SendsMessageExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Actions.FindWith<SendsMessageActionSource>();
            registry.Policies.Global.Add<SendsMessageConvention>();
        }
    }
}