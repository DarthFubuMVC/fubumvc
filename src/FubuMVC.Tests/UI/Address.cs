using System;

namespace FubuMVC.Tests.UI
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

        [Elements.FakeRequired, Elements.FakeMaximumStringLength(100)]
        public string AddressType { get; set; }

        [Elements.FakeRequired, Elements.FakeMaximumStringLength(250)]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Elements.FakeRequired, Elements.FakeMaximumStringLength(250)]
        public string City { get; set; }

        [Elements.FakeRequired]
        public string StateOrProvince { get; set; }

        [Elements.FakeRequired, Elements.FakeMaximumStringLength(100)]
        public string Country { get; set; }

        [Elements.FakeRequired, Elements.FakeMaximumStringLength(50)]
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