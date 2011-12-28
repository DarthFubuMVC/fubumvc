using System.IO;

namespace FubuMVC.OwinHost
{
    public class StreamDataReader : IOwinRequestReader
    {
        private readonly MemoryStream _stream;

        public StreamDataReader(MemoryStream stream)
        {
            _stream = stream;
        }

        public void Read(byte[] bytes, int offset, int count)
        {
            _stream.Write(bytes, offset, count);
        }

        public void Finish()
        {
            
        }
    }
}