using System;

namespace FubuCore.Binding
{
    public class FlatFileRequest<T>
    {
        public Action<T> Callback { get; set; }
        public Func<IRequestData, T> Finder { get; set; }
        public string Filename { get; set; }
        public string Concatenator { get; set; }
    }
}