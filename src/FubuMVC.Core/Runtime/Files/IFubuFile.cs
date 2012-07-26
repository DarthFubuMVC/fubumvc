using System;
using System.IO;

namespace FubuMVC.Core.Runtime.Files
{
    public interface IFubuFile
    {
        string Path { get; }
        string Provenance { get; }
        string RelativePath { get; set; }
        string ReadContents();
        void ReadContents(Action<Stream> action);
        void ReadLines(Action<string> read);
    }
}