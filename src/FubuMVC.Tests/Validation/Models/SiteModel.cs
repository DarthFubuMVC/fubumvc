using FubuMVC.Core.Validation;

namespace FubuMVC.Tests.Validation.Models
{
    public class UserModel
    {
        [Required]
        public string Login { get; set; }

        [ContinueValidation]
        public SiteModel Site { get; set; }
    }

    public class SiteModel
    {
        [Required]
        public string Name { get; set; }

        [ContinueValidation]
        public ContactModel Contact { get; set; }

        public ContactModel AlternateContact { get; set; }
    }
}