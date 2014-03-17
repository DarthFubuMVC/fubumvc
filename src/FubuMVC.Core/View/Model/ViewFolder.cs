using System.Collections.Generic;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateFolder
    {
        ITemplateFile FindInShared(string viewName);
        ITemplateFolder Parent { get; }
    }

    public class ViewFolder<T> where T : ITemplateFile
    {
        private readonly ViewFacility<T> _facility;
        private readonly string _name;
        public ViewFolder<T> Parent;
        public readonly IList<ViewFolder<T>> LayoutFolders = new List<ViewFolder<T>>(); 
        public bool IsShared;
        public readonly IList<T> Views = new List<T>();


        public ViewFolder(ViewFacility<T> facility, string name)
        {
            _facility = facility;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}