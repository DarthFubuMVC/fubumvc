using System;
using System.Linq;
using System.Security.Principal;

namespace FubuMVC.Core.Security.Authentication.Membership
{
    public class FubuPrincipal : IPrincipal
    {
        private readonly IUserInfo _user;
        private readonly Func<string, bool> _isInRole;

        public FubuPrincipal(IUserInfo user) : this(user, role => user.Roles.Contains(role))
        {
        }

        public FubuPrincipal(IUserInfo user, Func<string, bool> isInRole)
        {
            if (user == null) throw new ArgumentNullException("user");

            _user = user;
            _isInRole = isInRole;
            Identity = new GenericIdentity(user.UserName);
        }

        public IUserInfo User
        {
            get { return _user; }
        }

        public T Get<T>() where T : class
        {
            return _user.Get<T>();
        }

        public static FubuPrincipal Current
        {
            get
            {
                var context = new ThreadPrincipalContext();
                return context.Current as FubuPrincipal;
            }
        }

        public static void SetCurrent(Action<UserInfo> configuration)
        {
            var user = new UserInfo();
            configuration(user);

            var principal = new FubuPrincipal(user);
            new ThreadPrincipalContext().Current = principal;
        }

        public bool IsInRole(string role)
        {
            return _isInRole(role);
        }

        public IIdentity Identity { get; private set; }
    }
}