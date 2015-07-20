using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.Core.Security.Authentication.Membership;

namespace FubuMVC.PersistedMembership
{
    public class PersistedMembership<T> : IFubuRegistryExtension where T : User
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services(x =>
            {
                x.ReplaceService<IMembershipRepository, MembershipRepository<T>>();
                x.SetServiceIfNone<IPasswordHash, PasswordHash>();
                x.SetServiceIfNone<ILoginAuditor, PersistedLoginAuditor>();
            });

            registry.AlterSettings<AuthenticationSettings>(x =>
            {
                x.Strategies.AddToEnd(new MembershipNode(typeof (MembershipRepository<T>)));

                x.Strategies.OfType<MembershipNode>()
                    .Where(node => node.MembershipType == typeof (MembershipAuthentication))
                    .ToArray()
                    .Each(node => node.Remove());
            });
        }
    }
}