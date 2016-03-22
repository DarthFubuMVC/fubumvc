using System.Linq;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using Marten;

namespace FubuMVC.Marten.Membership
{
    public class MembershipRepository<T> : IMembershipRepository where T : User
    {
        private readonly IDocumentSession _repository;
        private readonly IPasswordHash _hash;

        public MembershipRepository(IDocumentSession repository, IPasswordHash hash)
        {
            _repository = repository;
            _hash = hash;
        }

        public bool MatchesCredentials(LoginRequest request)
        {
            var hashed = _hash.CreateHash(request.Password);
            return _repository.Query<T>().SingleOrDefault(x => x.UserName == request.UserName && x.Password == hashed) != null;
        }

        public IUserInfo FindByName(string username)
        {
            return _repository.Query<T>().SingleOrDefault(x => x.UserName == username);
        }
    }
}