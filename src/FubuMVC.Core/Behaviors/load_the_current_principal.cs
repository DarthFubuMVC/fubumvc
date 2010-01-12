using System.Security.Principal;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Behaviors
{
    public class load_the_current_principal : BasicBehavior
    {
        private readonly ISecurityContext _context;
        private readonly IPrincipalFactory _factory;

        public load_the_current_principal(ISecurityContext context, IPrincipalFactory factory)
            : base(PartialBehavior.Ignored)
        {
            _context = context;
            _factory = factory;
        }

        public void AttachPrincipal()
        {
            if (!_context.IsAuthenticated()) return;

            IIdentity identity = _context.CurrentIdentity;
            _context.CurrentUser = _factory.CreatePrincipal(identity);
        }

        protected override DoNext performInvoke()
        {
            AttachPrincipal();
            return DoNext.Continue;
        }
    }
}