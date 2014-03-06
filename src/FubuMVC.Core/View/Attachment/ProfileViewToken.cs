using System;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;

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

        public IViewToken View
        {
            get { return _view; }
        }

        public Type ViewType
        {
            get { return _view.ViewType; }
        }

        public Type ViewModel
        {
            get { return _view.ViewModel; }
        }

        public string Name()
        {
            return _filteredName;
        }

        public string Namespace
        {
            get { return _view.Namespace; }
        }

        public IRenderableView GetView()
        {
            throw new NotSupportedException();
        }

        public IRenderableView GetPartialView()
        {
            throw new NotSupportedException();
        }

        public string ProfileName { get; set; }
        public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
        {
            throw new NotSupportedException();
        }
    }
}