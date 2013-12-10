using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Alters the route generation in the default routing conventions
    /// to override the usual handling of the class name with the value of 
    /// the [UrlFolder] name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UrlFolderAttribute : Attribute
    {
        private readonly string _folder;

        public UrlFolderAttribute(string folder)
        {
            _folder = folder;
        }

        public string Folder
        {
            get { return _folder; }
        }
    }
}