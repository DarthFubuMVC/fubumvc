using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation
{
    [Serializable]
    public class NotificationMessage
    {
        private readonly List<Accessor> _accessors = new List<Accessor>();
        private readonly Template _template;
        private readonly Lazy<string> _message;

        public NotificationMessage(StringToken stringToken, params TemplateValue[] values)
        {
            StringToken = stringToken;

            _template = new Template(stringToken, values);
            _message = new Lazy<string>(() => _template.Render());
        }

        public NotificationMessage(string message)
        {
            _message = new Lazy<string>(() => message);
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
            return AddSubstitution(TemplateValue.For(key, value));
        }

        public NotificationMessage AddSubstitution(TemplateValue value)
        {
            if (_template == null) throw new InvalidOperationException("This NotificationMessage has no template");

            _template.Values.Add(value);
            return this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(NotificationMessage)) return false;
            return Equals((NotificationMessage)obj);
        }

        public bool Equals(NotificationMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return _template.Equals(other._template) && Accessors.IsEqualTo(other.Accessors);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (Accessors != null ? Accessors.GetHashCode() : 0);
                result = (result * 397) ^ (GetMessage() != null ? GetMessage().GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return Accessors.Select(x => x.Name).Join(", ") + ":  " + GetMessage();
        }

        public NotificationMessage Prepend(Accessor accessor)
        {
            var prependedAccessors = _accessors.Select(x => x.Prepend(accessor)).ToList();
            var message = new NotificationMessage(StringToken, _template.Values.ToArray());
            message._accessors.AddRange(prependedAccessors);

            return message;
        }

        public IEnumerable<ValidationError> ToValidationErrors()
        {
            var message = GetMessage();
            if (_accessors.Any())
            {
                return _accessors.Select(a => new ValidationError(a.Name, a.ToHeader(), message));
            }

            return new[] { new ValidationError(string.Empty, message) };
        }
    }
}