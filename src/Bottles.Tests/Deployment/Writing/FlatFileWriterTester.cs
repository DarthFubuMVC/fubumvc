using System.Collections.Generic;
using Bottles.Deployment.Writing;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests.Deployment.Writing
{
    [TestFixture]
    public class FlatFileWriterTester
    {
        private FlatFileWriter theWriter;

        [SetUp]
        public void SetUp()
        {
            theWriter = new FlatFileWriter(new List<string>());
        }

        [Test]
        public void write_property_value_once()
        {
            theWriter.WriteProperty("key", "value");

            theWriter.List.ShouldHaveTheSameElementsAs("key=value");
        }

        [Test]
        public void overwrite_property_value()
        {
            theWriter.WriteProperty("key", "value");
            theWriter.WriteProperty("key", "different");

            theWriter.List.ShouldHaveTheSameElementsAs("key=different");
        }

        [Test]
        public void write_multiple_properties_and_sort()
        {
            theWriter.WriteProperty("key2", "value2");
            theWriter.WriteProperty("key3", "value3");
            theWriter.WriteProperty("key1", "value1");

            theWriter.Sort();

            theWriter.List.ShouldHaveTheSameElementsAs("key1=value1", "key2=value2", "key3=value3");
        }

        [Test]
        public void write_line()
        {
            theWriter.WriteLine("bottle:one");
            theWriter.List.ShouldHaveTheSameElementsAs("bottle:one");
        }

        [Test]
        public void write_line_repeatedly_is_idempotent()
        {
            theWriter.WriteLine("bottle:one");
            theWriter.WriteLine("bottle:one");
            theWriter.WriteLine("bottle:two");
            theWriter.WriteLine("bottle:two");
            theWriter.WriteLine("bottle:one");
            theWriter.List.ShouldHaveTheSameElementsAs("bottle:one", "bottle:two");
        }
    }
}