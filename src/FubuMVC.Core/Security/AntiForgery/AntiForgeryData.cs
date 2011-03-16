using System;
using System.Security.Principal;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryData
    {
        private DateTime _creationDate = DateTime.UtcNow;
        private string _salt;
        private string _username;
        private string _value;

        public AntiForgeryData()
        {
        }

        public AntiForgeryData(AntiForgeryData token)
        {
            CreationDate = token.CreationDate;
            Salt = token.Salt;
            Username = token.Username;
            Value = token.Value;
        }

        public string Salt
        {
            get { return _salt ?? string.Empty; }
            set { _salt = value; }
        }

        public string Value
        {
            get { return _value ?? string.Empty; }
            set { _value = value; }
        }

        public string Username
        {
            get { return _username ?? string.Empty; }
            set { _username = value; }
        }

        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; }
        }

        public void SetUser(IPrincipal user)
        {
            Username = GetUsername(user);
        }

        public static string GetUsername(IPrincipal user)
        {
            if (user != null)
            {
                IIdentity identity = user.Identity;
                if (identity != null && identity.IsAuthenticated)
                {
                    return identity.Name;
                }
            }

            return String.Empty;
        }
    }
}