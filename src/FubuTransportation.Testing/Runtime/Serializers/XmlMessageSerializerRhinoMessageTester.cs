using System.IO;
using FubuTestingSupport;
using FubuTransportation.Runtime.Serializers;
using NUnit.Framework;

namespace FubuTransportation.Testing.Runtime.Serializers
{
    [TestFixture]
    public class XmlMessageSerializerRhinoMessageTester
    {
        private readonly XmlMessageSerializer _serializer;
        private readonly MemoryStream _stream;

        private const string XmlInput = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <esb:messages xmlns:esb=""http://servicebus.hibernatingrhinos.com/2008/12/20/esb""
                xmlns:commands.testrhinotoftserialization=""FubuTransportation.Testing.Runtime.Serializers.TestRhinoToFtSerialization, FubuTransportation.Testing""
                xmlns:System.Int32=""System.Int32"">
              <commands.testrhinotoftserialization:TestRhinoToFtSerialization>
                <System.Int32:Id>1</System.Int32:Id>
              </commands.testrhinotoftserialization:TestRhinoToFtSerialization>
            </esb:messages>";


        public XmlMessageSerializerRhinoMessageTester()
        {
            _serializer = new XmlMessageSerializer();
            _stream = new MemoryStream();
        }

        [Test]
        public void can_deserialize_rhino_esb_message()
        {
            var writer = new StreamWriter(_stream);
            writer.Write(XmlInput);
            writer.Flush();
            _stream.Position = 0;

            var result = (TestRhinoToFtSerialization) _serializer.Deserialize(_stream);
            result.Id.ShouldEqual(1);
        }
    }

    public class TestRhinoToFtSerialization
    {
        public int Id { get; set; }
    }
}