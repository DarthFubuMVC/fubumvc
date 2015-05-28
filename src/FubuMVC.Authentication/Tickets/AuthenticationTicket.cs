using System;

namespace FubuMVC.Authentication.Tickets
{
    public class AuthenticationTicket
    {
        public DateTime LastAccessed { get; set; }
        public DateTime Expiration { get; set; }
        public string UserName { get; set;}
        public string UserData { get; set; }
    }
}