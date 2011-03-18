using FubuCore;
using FubuValidation;
using FubuValidation.Registration;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
{
    [TestFixture]
    public class debugging
    {

    }

    public class ValidationTarget
    {
        [Required]
        public string Name { get; set; }
    }
}