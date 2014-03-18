using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateFolder
    {
        ITemplateFile FindRecursivelyInShared(string viewName);
        ITemplateFolder Parent { get; }
    }

    public class ViewFolder<T> : ITemplateFolder where T : class, ITemplateFile
    {
        private readonly string _name;
        private readonly string _path;
        public ViewFolder<T> Parent;
        public readonly IList<ViewFolder<T>> LayoutFolders = new List<ViewFolder<T>>(); 
        public bool IsShared;
        public readonly IList<T> Views = new List<T>();


        public ViewFolder(string path)
        {
            _name = System.IO.Path.GetFileNameWithoutExtension(path);
            _path = path;
        }

        public string Path
        {
            get { return _path; }
        }

        public string Name
        {
            get { return _name; }
        }

        public void AttachLayouts(string defaultLayoutName, ViewFacility<T> facility)
        {
            Views.Each(x => x.AttachLayouts(defaultLayoutName, facility, this));
        }

        public T FindInShared(string viewName)
        {
            return LayoutFolders.SelectMany(x => x.Views).FirstOrDefault(x => x.Name() == viewName);
        }

        public ITemplateFile FindRecursivelyInShared(string viewName)
        {
            var shared = FindInShared(viewName);
            if (shared == null && Parent != null)
            {
                if (IsShared && Parent != null && Parent.Parent != null)
                {
                    return Parent.Parent.As<ITemplateFolder>().FindRecursivelyInShared(viewName);
                }

                return Parent.As<ITemplateFolder>().FindRecursivelyInShared(viewName);
            }

            return shared;
        }

        ITemplateFolder ITemplateFolder.Parent
        {
            get { return IsShared ? Parent.Parent : Parent; }
        }

        public string RelativePath { get; set; }
    }
}