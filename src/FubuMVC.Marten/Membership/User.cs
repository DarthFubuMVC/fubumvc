using System;
using FubuMVC.Core.Security.Authentication.Membership;

namespace FubuMVC.Marten.Membership
{
    public class User : UserInfo
    {
        public Guid Id { get; set; }
    }
}