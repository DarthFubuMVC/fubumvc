namespace FubuMVC.Core.View.Model
{
    public interface ITemplateFile
    {
        string Origin { get; set; }
        string FilePath { get; set; }
        string RootPath { get; set; }
        string ViewPath { get; set; }
    }
}