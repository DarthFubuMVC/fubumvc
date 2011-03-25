using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Tests.Models
{
    public class SampleGridModel
    {
        public SampleGridModel()
        {
            Rows = new List<SampleGridRowModel>();
        }

        public IEnumerable<SampleGridRowModel> Rows { get; set; }
    }

    public class SampleGridRowModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}