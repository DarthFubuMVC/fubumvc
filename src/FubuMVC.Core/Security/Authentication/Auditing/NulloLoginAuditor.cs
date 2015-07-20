using System.Diagnostics;

namespace FubuMVC.Core.Security.Authentication.Auditing
{
    public class NulloLoginAuditor : ILoginAuditor
    {
        public void Audit(LoginRequest request)
        {
            Debug.WriteLine(request);
        }

        public void ApplyHistory(LoginRequest request)
        {
            // do nothing
        }

        public void Audit<T>(T log) where T : AuditMessage
        {
            Debug.WriteLine(log);
        }
    }
}