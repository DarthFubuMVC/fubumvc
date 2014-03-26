using System;
using System.Collections.Generic;

namespace FubuMVC.Tests.UI.Elements
{
    public class AddressViewModel
    {
        public Address Address { get; set; }
        public bool ShouldShow { get; set; }
        public IList<LocalityViewModel> Localities { get; set; }
    }

    public class LocalityViewModel
    {
        public string ZipCode { get; set; }
        public string CountyName { get; set; }
    }

    public class Address
    {
        public Address()
        {
            StateOrProvince = string.Empty;
            Country = string.Empty;
            AddressType = string.Empty;
        }

        public int Order { get; set; }

        public bool IsActive { get; set; }

        [FakeRequired, FakeMaximumStringLength(100)]
        public string AddressType { get; set; }

        [FakeRequired, FakeMaximumStringLength(250)]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [FakeRequired, FakeMaximumStringLength(250)]
        public string City { get; set; }

        [FakeRequired]
        public string StateOrProvince { get; set; }

        [FakeRequired, FakeMaximumStringLength(100)]
        public string Country { get; set; }

        [FakeRequired, FakeMaximumStringLength(50)]
        public string PostalCode { get; set; }

        //[Required]
        //public TimeZoneInfo TimeZone { get; set; }


        public DateTime DateEntered { get; set; }

        public ColorEnum Color { get; set; }
        public Guid Guid { get; set; }
    }

    public class FakeRequiredAttribute : Attribute
    {
    }

    public class FakeMaximumStringLength : Attribute
    {
        private readonly int _maxLength;

        public FakeMaximumStringLength(int maxLength)
        {
            _maxLength = maxLength;
        }

        public int MaxLength { get { return _maxLength; } }
    }

    public enum ColorEnum
    {
        red, blue, green
    }
}