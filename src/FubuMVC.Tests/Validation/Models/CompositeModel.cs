using System.Collections.Generic;
using FubuMVC.Core.Validation;

namespace FubuMVC.Tests.Validation.Models
{
    public class CompositeModel
    {
        public int Id { get; set; }
        [ContinueValidation]
        public ContactModel Contact { get; set; }
        public ContactModel RestrictedContact { get; set; }
        public IList<ContactModel> Contacts { get; set; }
    }
}