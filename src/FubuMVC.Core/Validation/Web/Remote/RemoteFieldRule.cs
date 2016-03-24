using System;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.Remote
{
    public class RemoteFieldRule
    {
		private readonly IFieldValidationRule _rule;
        private readonly Accessor _accessor;

        public RemoteFieldRule(IFieldValidationRule rule, Accessor accessor)
        {
	        _rule = rule;
            _accessor = accessor;
        }

        public Accessor Accessor
        {
            get { return _accessor; }
        }

	    public IFieldValidationRule Rule
	    {
			get { return _rule; }
	    }

        public Type Type
        {
            get { return _rule.GetType(); }
        }

        public string ToHash()
        {
			return "RuleType={0}&Type={1}&Accessor={2}".ToFormat(Type.FullName, _accessor.OwnerType.FullName, _accessor.Name).ToHash();
        }

		public bool Matches(string hash)
		{
			return ToHash().Equals(hash);
		}

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RemoteFieldRule)) return false;
            return Equals((RemoteFieldRule) obj);
        }

        public bool Equals(RemoteFieldRule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _rule.Equals(other._rule) && Equals(other._accessor, _accessor);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode()*397) ^ _accessor.GetHashCode();
            }
        }

        public static RemoteFieldRule For(Accessor accessor, IFieldValidationRule rule)
        {
            return new RemoteFieldRule(rule, accessor);
        }

		public static RemoteFieldRule For<T>(Accessor accessor)
			where T : IFieldValidationRule, new()
		{
			return new RemoteFieldRule(new T(), accessor);
		}
    }
}
