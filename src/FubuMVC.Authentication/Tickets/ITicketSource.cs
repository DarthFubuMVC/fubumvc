namespace FubuMVC.Authentication.Tickets
{
    public interface ITicketSource
    {
        AuthenticationTicket CurrentTicket();
        void Persist(AuthenticationTicket ticket);
        void Delete();
    }
}