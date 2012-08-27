using System;
using System.IO;

namespace FubuMVC.Core.Runtime
{
    internal interface IOutputState
    {
        void Write(string contentType, string renderedOutput);
        void Write(string contentType, Action<Stream> action);

        void AppendHeader(string header, string value);
        void WriteFile(string contentType, string localFilePath, string displayName);
        void Flush();
        void Write(string renderedOutput);
    }
}