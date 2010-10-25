using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuLocalization;

namespace FubuValidation
{
    [Serializable]
    public class NotificationMessage
    {
        private string _message;
        private readonly List<PropertyInfo> _properties = new List<PropertyInfo>();
        private readonly Dictionary<string, string> _messageSubstitutions = new Dictionary<string, string>();

        public NotificationMessage(StringToken stringToken)
        {
            StringToken = stringToken;
        }

        public string Message
        {
            get
            {
                if(_message == null)
                {
                    var localizedMessage = StringToken.ToString();
                    _message = TemplateParser.Parse(localizedMessage, _messageSubstitutions);
                }

                return _message;
            }
        }

        public StringToken StringToken { get; private set; }
        public PropertyInfo[] Properties { get { return _properties.ToArray(); } }
        public IEnumerable<KeyValuePair<string, string>> MessageSubstitutions
        {
            get
            {
                return _messageSubstitutions
                    .Keys
                    .Select(key => new KeyValuePair<string, string>(key, _messageSubstitutions[key]));
            }
        }

        public void AddProperty(PropertyInfo property)
        {
            _properties.Fill(property);
        }

        public void AddSubstitution(string key, string value)
        {
            _messageSubstitutions.Fill(key, value);
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

            return Equals(other.Message, Message) && Properties.IsEqualTo(other.Properties);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Properties != null ? Properties.GetHashCode() : 0);
                result = (result*397) ^ (Message != null ? Message.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return Message;
        }
    }
}