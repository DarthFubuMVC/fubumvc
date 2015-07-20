using System;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuPersistence;

namespace FubuMVC.PersistedMembership
{
    public class User : UserInfo, IEntity
    {
        public Guid Id { get; set; }
    }
}