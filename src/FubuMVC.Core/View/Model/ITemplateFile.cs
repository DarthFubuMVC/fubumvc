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

        ITemplateDescriptor Descriptor { get; set; }

        string Namespace { get;  }
        Type ViewModel { get; set; }
        Parsing Parsing { get; }
        string RelativePath();
        string DirectoryPath();
        string RelativeDirectoryPath();
        string Name();

        bool FromHost();
        bool IsPartial();

        string FullName();
    }
}