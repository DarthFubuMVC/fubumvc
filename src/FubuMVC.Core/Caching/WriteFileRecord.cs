using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public class WriteFileRecord : IRecordedHttpOutput, DescribesItself
    {
        private IList<Action<IHttpWriter>> _writes = new List<Action<IHttpWriter>>();

        private Action<IHttpWriter> write
        {
            set
            {
                _writes.Add(value);
            }
        }

        public static WriteFileRecord Create(IFileSystem fileSystem, string file, string contentType, string displayName)
        {
            var record = new WriteFileRecord();
            record.write = x => x.WriteContentType(contentType);
            
            if (displayName != null)
            {
                record.write = x => x.AppendHeader(HttpResponseHeaders.ContentDisposition, "attachment; filename=\"" + displayName + "\"");
            }

            var fileLength = fileSystem.FileSizeOf(file);
            record.write = x => x.AppendHeader(HttpResponseHeaders.ContentLength, fileLength.ToString());

            record.write = x => x.WriteFile(file);

            return record;
        }

        private WriteFileRecord()
        {
        }

        public void Replay(IHttpWriter writer)
        {
            _writes.Each(x => x(writer));
        }

        public void Describe(Description description)
        {
            description.Title = "Write file";
        }
    }
}