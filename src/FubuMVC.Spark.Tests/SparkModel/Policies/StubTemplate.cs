using System;
using System.Reflection;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark.Tests.SparkModel.Policies
{
    public class StubTemplate : ISparkTemplate
    {
        public string Origin
        {
            get; set;
        }

        public string FilePath
        {
            get;
            set;
        }

        public string RootPath
        {
            get;
            set;
        }

        public string ViewPath
        {
            get;
            set;
        }


        public string Namespace { get; set; }
        public Type ViewModel { get; set; }
        public string ProfileName { get; set; }

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

        public string Name()
        {
            throw new NotImplementedException();
        }

        public bool FromHost()
        {
            throw new NotImplementedException();
        }

        public bool IsPartial()
        {
            throw new NotImplementedException();
        }

        public bool HasViewModel()
        {
            throw new NotImplementedException();
        }

        public string FullName()
        {
            throw new NotImplementedException();
        }

        public void AttachViewModels(Assembly defaultAssembly, ViewTypePool types, ITemplateLogger logger)
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