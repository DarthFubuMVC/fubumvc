using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public class FileRead : IContentSource
    {
        private readonly AssetFile _file;

        public FileRead(AssetFile file)
        {
            _file = file;
        }

        public string GetContent(IContentPipeline pipeline)
        {
            return pipeline.ReadContentsFrom(_file.FullPath);
        }

        public IEnumerable<AssetFile> Files
        {
            get { yield return _file; }
        }

        public IEnumerable<IContentSource> InnerSources
        {
            get
            {
                yield break;
            }
        }

        public bool Equals(FileRead other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._file, _file);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FileRead)) return false;
            return Equals((FileRead) obj);
        }

        public override int GetHashCode()
        {
            return (_file != null ? _file.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return "FileRead:" + _file.Name;
        }
    }
}