using System;

namespace FubuMVC.RavenDb.Membership
{
    public class LoginFailureHistory
    {
        public string Id { get; set; }
        public int Attempts { get; set; }
        public DateTime? LockedOutTime { get; set; }
    }
}