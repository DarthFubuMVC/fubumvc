using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SubjectConfirmation : ReadsSamlXml
    {
        private readonly IList<SubjectConfirmationData> _data = new List<SubjectConfirmationData>();

        public SubjectConfirmation()
        {
        }

        public SubjectConfirmation(XmlElement element)
        {
            Method = element.GetAttribute(MethodAtt).ToUri();
            Name = new SamlName(element);
            ConfirmationData = buildData(element).ToArray();
        }

        public Uri Method { get; set; }

        public SamlName Name { get; set; }

        public SubjectConfirmationData[] ConfirmationData
        {
            get { return _data.ToArray(); }
            set
            {
                _data.Clear();
                _data.AddRange(value);
            }
        }

        private IEnumerable<SubjectConfirmationData> buildData(XmlElement element)
        {
            foreach (XmlElement dataElement in element.GetElementsByTagName(SubjectConfirmationData, AssertionXsd))
            {
                yield return new SubjectConfirmationData(dataElement);
            }
        }

        protected bool Equals(SubjectConfirmation other)
        {
            return Equals(Method, other.Method) && Equals(Name, other.Name) &&
                   ConfirmationData.SequenceEqual(other.ConfirmationData);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SubjectConfirmation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Method != null ? Method.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ConfirmationData != null ? ConfirmationData.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void Add(SubjectConfirmationData data)
        {
            _data.Add(data);
        }
    }
}