using System.Collections.Generic;
using System.Diagnostics;
using FubuValidation;
using FubuValidation.Strategies;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
{
    [TestFixture]
    public class debugging
    {
        [Test]
        public void print_class_names()
        {
            typeof (IFieldValidationStrategy).Assembly.GetExportedTypes().Each(x => { Debug.WriteLine(x.FullName); });
        }
    }

    public class ValidationTarget
    {
        [Required]
        public string Name { get; set; }
    }
}