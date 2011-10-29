using System.IO;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public class WriteStream : IRecordedHttpOutput
    {
        private readonly MemoryStream _stream;
        private readonly object _locker = new object();

        public WriteStream(MemoryStream stream)
        {
            _stream = stream;
        }

        public void Replay(IHttpWriter writer)
        {
            writer.Write(stream =>
            {
                lock (_locker)
                {
                    _stream.Position = 0;
                    _stream.CopyTo(stream);
                }
            });
        }

        public string ReadAll()
        {
            lock (_locker)
            {
                _stream.Position = 0;
                var reader = new StreamReader(_stream);
                return reader.ReadToEnd();
            }
        }
    }
}