using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication.Membership;

namespace FubuMVC.Core.Security.Authentication
{
    [Description("Registers the authentication strategies to the application container")]
    public class RegisterAuthenticationStrategies : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<AuthenticationSettings>();

            if (settings.MembershipEnabled == MembershipStatus.Enabled)
            {
                if (!settings.Strategies.OfType<MembershipNode>().Any())
                {
                    settings.Strategies.InsertFirst(new MembershipNode());
                }
            }



            foreach (IContainerModel strategy in settings.Strategies)
            {
                var def = strategy.ToObjectDef();
                graph.Services.AddService(typeof (IAuthenticationStrategy), def);
            }
        }
    }
}