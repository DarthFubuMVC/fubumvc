using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View.Model
{
    // TODO -- look to see how much of this can be thinned down
    public interface ITemplateFile : IViewToken
    {
        string FilePath { get;  }
        string RootPath { get; }
        string ViewPath { get; }

        Parsing Parsing { get; }

        string RelativePath();
        string DirectoryPath();
        string RelativeDirectoryPath();

        bool FromHost();
        bool IsPartial();

        string FullName();

        ITemplateFile Master { get; set; }

        IRenderableView GetView();
        IRenderableView GetPartialView();

        void AttachLayouts(string defaultLayoutName, IViewFacility facility, ITemplateFolder folder);
    }
}