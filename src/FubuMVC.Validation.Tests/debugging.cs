using System.Collections.Generic;
using System.Diagnostics;
using FubuValidation;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
{

    public class ValidationTarget
    {
        [Required]
        public string Name { get; set; }
    }
}