using System;
using FubuMVC.Core.UI.Testing.Elements;

namespace FubuMVC.Core.UI.Testing
{
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

    public enum ColorEnum
    {
        red, blue, green
    }
}