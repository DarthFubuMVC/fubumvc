namespace FubuValidation.Tests
{
    public class SimpleModel
    {
        [GreaterOrEqualToZero]
        public int GreaterOrEqualToZero { get; set; }
        [GreaterThanZero]
        public int GreaterThanZero { get; set; }
        [MaximumStringLength(10)]
        public int NoMoreThanTen { get; set; }
    }
}