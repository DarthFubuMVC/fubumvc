using System;

namespace FubuMVC.Diagnostics.Features.Html
{
    public class HtmlConventionsPreviewRequestModel
    {
        public string OutputModel { get; set; }
    }

    public class HtmlConventionsPreviewInputModel
    {
        public string OutputModel { get; set; }
    }

    public class ExampleViewModel
    {
        public Person Person { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public Address HomeAddress { get; set; }
        public int Age { get; set; }
        public bool Married { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }
}