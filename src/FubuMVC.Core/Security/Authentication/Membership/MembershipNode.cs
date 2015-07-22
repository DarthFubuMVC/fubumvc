using System;
using FubuCore;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Security.Authentication.Membership
{
    public class MembershipNode : AuthenticationNode
    {
        private readonly Type _membershipType;

        public MembershipNode() : this(null)
        {
        }

        public MembershipNode(Type membershipType) : base(typeof (MembershipAuthentication))
        {
            if (membershipType != null && !membershipType.CanBeCastTo<IMembershipRepository>())
            {
                throw new ArgumentOutOfRangeException("membershipType",
                                                      "membershipType has to be assignable to IMembershipRepository");
            }

            _membershipType = membershipType;
        }

        public Type MembershipType
        {
            get { return _membershipType; }
        }

        protected override void configure(IConfiguredInstance instance)
        {
            if (_membershipType != null)
            {
                instance.Dependencies.Add(typeof(IMembershipRepository), new ConfiguredInstance(_membershipType));
            }
        }

        public static MembershipNode For<T>() where T : IMembershipRepository
        {
            return new MembershipNode(typeof (T));
        }
    }
}