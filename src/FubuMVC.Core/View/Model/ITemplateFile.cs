namespace FubuMVC.Core.View.Model
{
    public interface ITemplateFile
    {
        string Origin { get; }
        string FilePath { get; }
        string RootPath { get; }
    }
}