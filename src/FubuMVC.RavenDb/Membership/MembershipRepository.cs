using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;

namespace FubuMVC.RavenDb.Membership
{
    public class MembershipRepository<T> : IMembershipRepository where T : User
    {
        private readonly IEntityRepository _repository;
        private readonly IPasswordHash _hash;

        public MembershipRepository(IEntityRepository repository, IPasswordHash hash)
        {
            _repository = repository;
            _hash = hash;
        }

        public bool MatchesCredentials(LoginRequest request)
        {
            var hashed = _hash.CreateHash(request.Password);
            return _repository.FindWhere<T>(x => x.UserName == request.UserName && x.Password == hashed) != null;
        }

        public IUserInfo FindByName(string username)
        {
            return _repository.FindWhere<T>(x => x.UserName == username);
        }
    }
}