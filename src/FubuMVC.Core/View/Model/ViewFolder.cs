using System.Collections.Generic;

namespace FubuMVC.Core.View.Model
{
    public class ViewFolder<T> where T : ITemplateFile
    {
        public ViewFolder<T> Parent;
        public readonly IList<ViewFolder<T>> LayoutFolders = new List<ViewFolder<T>>(); 
        public bool IsShared;
        public readonly IList<T> Views = new List<T>(); 
    }
}