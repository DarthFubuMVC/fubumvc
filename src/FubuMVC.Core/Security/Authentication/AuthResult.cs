using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthResult
    {
        public bool Success;
        public FubuContinuation Continuation;

        public bool IsDeterministic()
        {
            return Success || Continuation != null;
        }

        public static AuthResult Failed()
        {
            return new AuthResult{Success = false};
        }

        public static AuthResult Successful()
        {
            return new AuthResult{Success = true};
        }

        public override string ToString()
        {
            if (Continuation != null) return string.Format("Success: {0}, Continuation: {1}", Success, Continuation);

            return "Success: " + Success;
        }
    }
}