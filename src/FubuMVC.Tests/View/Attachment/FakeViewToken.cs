using System;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Tests.View.Attachment
{
    public class FakeViewToken : IViewToken, ITemplateFile
    {
        public string Namespace { get; set; }
        public Type ViewModel { get; set; }
        public string ProfileName { get; set; }

        public string ViewName { get; set; }
        public string Name()
        {
            return ViewName;
        }

        public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
        {
            throw new NotImplementedException();
        }

        public string FilePath { get; set; }
        public string ViewPath { get; private set; }
        public Parsing Parsing { get; private set; }
        public string RelativePath()
        {
            throw new NotImplementedException();
        }

        public string DirectoryPath()
        {
            throw new NotImplementedException();
        }

        public string RelativeDirectoryPath()
        {
            throw new NotImplementedException();
        }

        public bool IsPartial()
        {
            throw new NotImplementedException();
        }

        public ITemplateFile Master { get; set; }
        public IRenderableView GetView()
        {
            throw new NotImplementedException();
        }

        public IRenderableView GetPartialView()
        {
            throw new NotImplementedException();
        }

        public void AttachLayouts(string defaultLayoutName, IViewFacility facility, ITemplateFolder folder)
        {
            throw new NotImplementedException();
        }
    }
}