using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace AssemblyPackage
{
    public class AddressOverrides : OverridesFor<Address>
    {
        public AddressOverrides()
        {
            Property(x => x.Address1).Add(new ElementRule("1")).Add(new ElementRule("2"));
            Property(x => x.Address2).Add(new ElementRule("3")).Add(new ElementRule("4"));
        }
    }

    public class ElementRule
    {
        private readonly string _name;

        public ElementRule(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        protected bool Equals(ElementRule other)
        {
            return string.Equals(_name, other._name);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementRule) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }
    }


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