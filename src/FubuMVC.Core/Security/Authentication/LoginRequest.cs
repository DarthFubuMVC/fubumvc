using System;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Security.Authentication
{
    [NotAuthenticated]
    public class LoginRequest
    {
        private readonly Cache<string, string> _properties = new Cache<string, string>();
        private string _userName;

        public LoginRequest()
        {
            Status = LoginStatus.NotAuthenticated;
        }

        [QueryString]
        public string Url { get; set; }

        [QueryString]
        public string ReturnUrl
        {
            set { Url = value; }
            get { return Url; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value == null ? null : value.ToLowerInvariant(); }
        }

        public string Password { get; set; }
        public int NumberOfTries { get; set; }

        public LoginStatus Status { get; set; }

        public string Message { get; set; }
        public bool RememberMe { get; set; }

        public DateTime? LockedOutUntil { get; set; }

        public string Get(string key)
        {
            return _properties[key];
        }

        public void Set(string key, string value)
        {
            _properties[key] = value;
        }

        public bool HasCredentials()
        {
            return UserName.IsNotEmpty() && Password.IsNotEmpty();
        }

        protected bool Equals(LoginRequest other)
        {
            return string.Equals(Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LoginRequest) obj);
        }

        public override int GetHashCode()
        {
            return (Url != null ? Url.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Login request for UserName: {0}, Status: {1}, NumberOfTries: {2}, Message: {3}",
                                 _userName, Status, NumberOfTries, Message);
        }
    }
}