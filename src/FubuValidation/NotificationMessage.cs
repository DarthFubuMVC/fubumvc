using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuValidation
{
    [Serializable]
    public class NotificationMessage
    {
        private readonly List<Accessor> _accessors = new List<Accessor>();
        private readonly Dictionary<string, string> _messageSubstitutions;
        private readonly Lazy<string> _message;

        private NotificationMessage(StringToken stringToken, Dictionary<string, string> messageSubstitutions)
        {
            _messageSubstitutions = messageSubstitutions;
            StringToken = stringToken;
            _message = new Lazy<string>(() =>
            {
                var localizedMessage = StringToken.ToString();
                return TemplateParser.Parse(localizedMessage, _messageSubstitutions);
            });
        }

        public NotificationMessage(StringToken stringToken) : this(stringToken, new Dictionary<string, string>())
        {

        }





        public StringToken StringToken { get; private set; }

        public Accessor[] Accessors
        {
            get { return _accessors.ToArray(); }
        }

        public string GetMessage()
        {
            return _message.Value;
        }

        public void AddAccessor(Accessor accessor)
        {
            _accessors.Fill(accessor);
        }

        public NotificationMessage AddSubstitution(string key, string value)
        {
            _messageSubstitutions.Fill(key, value);
            return this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (NotificationMessage)) return false;
            return Equals((NotificationMessage) obj);
        }

        public bool Equals(NotificationMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(other.GetMessage(), GetMessage()) && Accessors.IsEqualTo(other.Accessors);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (Accessors != null ? Accessors.GetHashCode() : 0);
                result = (result*397) ^ (GetMessage() != null ? GetMessage().GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return GetMessage();
        }

        public NotificationMessage Prepend(PropertyInfo property)
        {
            var prependedAccessors = _accessors.Select(x => x.Prepend(property)).ToList();
            var message = new NotificationMessage(StringToken, _messageSubstitutions);
            message._accessors.AddRange(prependedAccessors);

            return message;
        }
    }
}