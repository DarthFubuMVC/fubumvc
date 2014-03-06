using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.View.Model
{
    public class Parsing
    {
        public Parsing()
        {
            Namespaces = Enumerable.Empty<string>();
        }

        public string ViewModelType { get; set; }
        public string Master { get; set; }
        public IEnumerable<string> Namespaces { get; set; }
    }

    public interface ITemplateFile
    {
        string FilePath { get;  }
        string RootPath { get; }
        string Origin { get; }
        string ViewPath { get; }

        string Namespace { get;  }

        // TODO -- tighten this up
        Type ViewModel { get; set; }

        // TODO -- hide this.
        Parsing Parsing { get; }

        string RelativePath();
        string DirectoryPath();
        string RelativeDirectoryPath();
        string Name();

        bool FromHost();
        bool IsPartial();

        string FullName();

        void AttachViewModels(ViewTypePool types, ITemplateLogger logger);

        ITemplateFile Master { get; set; }
    }
}