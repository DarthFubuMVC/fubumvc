using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FubuMVC.Core;
using NUnit.Framework;
using System.Linq;
using FubuCore;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class CustomExceptionsTester
    {
        [Test]
        public void all_exceptions_should_be_marked_as_serializable()
        {
            const string failureMessage = @"All custom exceptions must be marked as [Serializable]. The following Exceptions were found without the attribute: 
    {0}

Additionally, if the custom exception has properties, you may want individual tests that verify the data is properly serialized across a boundary.";

            var assembliesToCheck = AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.FullName.Contains("Fubu") && !asm.FullName.Contains(".Test"));
            var typesToCheck = assembliesToCheck.SelectMany(asm => asm.GetExportedTypes().Where(t => t.CanBeCastTo<Exception>()));
            var notMarkedSerializable = typesToCheck.Where(t => !t.GetCustomAttributes(typeof (SerializableAttribute), false).Any()).ToArray();
            if (notMarkedSerializable.Any())
            {
                Assert.Fail(failureMessage + "\n" + String.Join("\n\t", notMarkedSerializable.Select(t => t.FullName).ToArray()));
            }
        }

        public static T TransferViaSerialization<T>(T instance)
        {
            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, instance);
            stream.Position = 0;

            return (T)new BinaryFormatter().Deserialize(stream);
        }

    }
}