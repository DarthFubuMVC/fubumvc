namespace FubuMVC.Core.Security.Authentication.Auditing
{
    public interface ILoginAuditor
    {
        void Audit(LoginRequest request); // This is responsible for storing login status

        void ApplyHistory(LoginRequest request);
        void Audit<T>(T log) where T : AuditMessage;
    }
}