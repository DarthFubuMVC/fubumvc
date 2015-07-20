using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuMVC.Core.Security.Authentication.Membership
{
    public interface IUserInfo
    {
        string UserName { get; }
        IEnumerable<string> Roles { get; }

        T Get<T>() where T : class;
    }

    public class UserInfo : IUserInfo
    {
        private readonly IList<string> _roles = new List<string>();
        private readonly Cache<Type, object> _values = new Cache<Type, object>();

        public string Password { get; set; }
        public string UserName { get; set; }

        public IEnumerable<string> Roles
        {
            get { return _roles; }
            set
            {
                _roles.Clear();
                _roles.AddRange(value);
            }
        }

        public T Get<T>() where T : class
        {
            return _values[typeof (T)] as T;
        }
        
        public void Set<T>(T value) where T : class
        {
            _values[typeof (T)] = value;
        }

        public void AddRoles(params string[] roles)
        {
            roles.Each(r => _roles.Fill(r));
        }

        public bool Matches(LoginRequest request)
        {
            return UserName == request.UserName && Password == request.Password;
        }

        protected bool Equals(UserInfo other)
        {
            return string.Equals(UserName, other.UserName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserInfo) obj);
        }

        public override int GetHashCode()
        {
            return (UserName != null ? UserName.GetHashCode() : 0);
        }
    }
}