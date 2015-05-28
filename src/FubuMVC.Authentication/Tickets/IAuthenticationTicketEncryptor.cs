namespace FubuMVC.Authentication.Tickets
{
    public interface IAuthenticationTicketEncryptor
    {
        string Encrypt(AuthenticationTicket ticket);
        AuthenticationTicket Decrypt(string text);
    }
}