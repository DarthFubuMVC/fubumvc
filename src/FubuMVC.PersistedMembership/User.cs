using System;
using FubuMVC.Authentication.Membership;
using FubuPersistence;
using System.Linq;

namespace FubuMVC.PersistedMembership
{
    public class User : UserInfo, IEntity
    {
        public Guid Id { get; set; }
    }
}