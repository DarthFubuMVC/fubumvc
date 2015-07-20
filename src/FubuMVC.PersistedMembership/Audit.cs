using System;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuPersistence;

namespace FubuMVC.PersistedMembership
{
    public class Audit : Entity
    {
        public AuditMessage Message { get; set; }
        public DateTime Timestamp { get; set; }

        public string Type
        {
            get { return Message.GetType().Name; }
            set
            {
                // no-op;
            }
        }

        public string Username
        {
            get { return Message.UserName; }
            set
            {
                // do nothing
            }
        }
    }
}