using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View.Model
{
    // TODO -- look to see how much of this can be thinned down
    public interface ITemplateFile : IViewToken
    {
        string FilePath { get;  }
        string RootPath { get; }
        string Origin { get; }
        string ViewPath { get; }

        // TODO -- tighten this up

        // TODO -- hide this.
        Parsing Parsing { get; }

        string RelativePath();
        string DirectoryPath();
        string RelativeDirectoryPath();

        bool FromHost();
        bool IsPartial();

        string FullName();

        void AttachViewModels(ViewTypePool types, ITemplateLogger logger);

        ITemplateFile Master { get; set; }

        IRenderableView GetView();
        IRenderableView GetPartialView();
    }
}