using System;
using System.Reflection;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Core.View.Attachment
{
    public class ProfileViewToken : IViewToken
    {
        private readonly IViewToken _view;
        private readonly string _filteredName;

        public ProfileViewToken(IViewToken view, string filteredName)
        {
            _view = view;
            _filteredName = filteredName;
        }

        public string Origin
        {
            get { return _view.Origin; }
        }

        public IViewToken View
        {
            get { return _view; }
        }

        public Type ViewModel
        {
            get { return _view.ViewModel; }
        }

        public string Name()
        {
            return _filteredName;
        }

        public void AttachViewModels(Assembly defaultAssembly, ViewTypePool types, ITemplateLogger logger)
        {
            _view.AttachViewModels(defaultAssembly, types, logger);
        }

        public string FilePath
        {
            get
            {
                return _view.FilePath;
            }
        }

        public string Namespace
        {
            get { return _view.Namespace; }
        }


        public string ProfileName { get; set; }
    }
}