using System;
using System.IO;
using System.Threading.Tasks;

namespace FubuMVC.Core.Runtime
{
    internal interface IOutputState
    {
        Task Write(string contentType, string renderedOutput);
        Task Write(string contentType, Func<Stream, Task> action);

        void AppendHeader(string header, string value);

        // TODO -- this should be async someday
        void WriteFile(string contentType, string localFilePath, string displayName);

        // TODO -- this should be async someday
        void Flush();


        Task Write(string renderedOutput);
    }
}