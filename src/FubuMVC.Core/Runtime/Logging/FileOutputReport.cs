using System;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.Runtime.Logging
{
    public class FileOutputReport : LogRecord, IHaveContentType, DescribesItself
    {
        public string ContentType { get; set;}
        public string LocalFilePath;
        public string DisplayName;

        public bool Equals(FileOutputReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ContentType, ContentType) && Equals(other.LocalFilePath, LocalFilePath) && Equals(other.DisplayName, DisplayName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(FileOutputReport)) return false;
            return Equals((FileOutputReport)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (ContentType != null ? ContentType.GetHashCode() : 0);
                result = (result * 397) ^ (LocalFilePath != null ? LocalFilePath.GetHashCode() : 0);
                result = (result * 397) ^ (DisplayName != null ? DisplayName.GetHashCode() : 0);
                return result;
            }
        }

        public void Describe(Description description)
        {
            description.Title = "Wrote file " + LocalFilePath;
            description.Properties["ContentType"] = ContentType;
            description.Properties["LocalFilePath"] = LocalFilePath;
            description.Properties["DisplayName"] = DisplayName;
        }

        public override string ToString()
        {
            return string.Format("ContentType: {0}, LocalFilePath: {1}, DisplayName: {2}", ContentType, LocalFilePath, DisplayName);
        }
    }
}