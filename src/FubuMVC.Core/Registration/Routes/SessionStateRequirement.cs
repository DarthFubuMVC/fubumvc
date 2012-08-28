namespace FubuMVC.Core.Registration.Routes
{
    public class SessionStateRequirement
    {
        public static readonly SessionStateRequirement RequiresSessionState = new SessionStateRequirement();
        public static readonly SessionStateRequirement DoesNotUseSessionState = new SessionStateRequirement();

        private SessionStateRequirement()
        {
        }
    }
}