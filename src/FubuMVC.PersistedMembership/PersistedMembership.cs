using FubuMVC.Authentication;
using FubuMVC.Authentication.Auditing;
using FubuMVC.Authentication.Membership;
using FubuMVC.Core;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.PersistedMembership
{
    public class PersistedMembership<T> : IFubuRegistryExtension where T : User
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services(x => {
                x.ReplaceService<IMembershipRepository, MembershipRepository<T>>();
                x.SetServiceIfNone<IPasswordHash, PasswordHash>();
                x.SetServiceIfNone<ILoginAuditor, PersistedLoginAuditor>();
            });

            registry.AlterSettings<AuthenticationSettings>(x => {
                x.Strategies.AddToEnd(new MembershipNode(typeof(MembershipRepository<T>)));

                x.Strategies.OfType<MembershipNode>()
                 .Where(node => node.MembershipType == typeof (MembershipAuthentication))
                 .ToArray()
                 .Each(node => node.Remove());
            });
        }
    }
}