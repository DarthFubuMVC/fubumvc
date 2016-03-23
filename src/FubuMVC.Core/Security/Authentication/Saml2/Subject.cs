using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    // TODO -- add Method!
    public class Subject : ReadsSamlXml
    {
        public Subject()
        {
        }

        public Subject(XmlElement element)
        {
            Name = new SamlName(element);
            Confirmations = buildConfirmations(element).ToArray();
        }

        private IEnumerable<SubjectConfirmation> buildConfirmations(XmlElement element)
        {
            foreach (XmlElement confirmationElement in element.GetElementsByTagName(SubjectConfirmation, AssertionXsd))
            {
                yield return new SubjectConfirmation(confirmationElement);
            }
        } 

        public SamlName Name { get; set; }
    
        // can have multiple confirmations

        private readonly IList<SubjectConfirmation> _confirmations = new List<SubjectConfirmation>(); 

        public SubjectConfirmation[] Confirmations
        {
            get { return _confirmations.ToArray(); }
            set
            {
                _confirmations.Clear();
                _confirmations.AddRange(value);
            }
        }

        public void Add(SubjectConfirmation confirmation)
        {
            _confirmations.Add(confirmation);
        }
    }
}