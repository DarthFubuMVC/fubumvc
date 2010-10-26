namespace FubuValidation.Tests.Rules
{
    public class AddressModel
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }

    public class SimpleModel
    {
        public int GreaterOrEqualToZero { get; set; }
        public int GreaterThanZero { get; set; }
    }
}