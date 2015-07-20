using FubuCore.Util;

namespace FubuMVC.Core.Security.Authentication.Membership
{
    public class InMemoryMembershipRepository : IMembershipRepository
    {
        private readonly Cache<string,UserInfo> _users = new Cache<string, UserInfo>(name => new UserInfo{UserName = name}); 

        public bool MatchesCredentials(LoginRequest request)
        {
            return _users[request.UserName].Password == request.Password;
        }

        public IUserInfo FindByName(string username)
        {
            return _users[username];
        }

        public void Add(UserInfo user)
        {
            _users[user.UserName] = user;
        }

        public void Clear()
        {
            _users.ClearAll();
        }
    }
}