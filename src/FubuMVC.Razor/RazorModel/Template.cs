namespace FubuMVC.Razor.RazorModel
{
    public interface ITemplate
    {
        string FilePath { get; }
        string RootPath { get; }
        string Origin { get; }
		
        string ViewPath { get; set; }
        IRazorDescriptor Descriptor { get; set; }
    }

    public class Template : ITemplate
    {
        public Template(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;
            Descriptor = new NulloDescriptor();
        }

        public string FilePath { get; private set; }
        public string RootPath { get; private set; }
        public string Origin { get; private set; }
		
        public string ViewPath { get; set; }
        public IRazorDescriptor Descriptor { get; set; }

	    public override string ToString()
        {
            return FilePath;
        }
    }
}