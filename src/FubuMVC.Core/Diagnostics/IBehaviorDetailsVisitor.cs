using System.Data;
using FubuMVC.Core.Diagnostics.Tracing;

namespace FubuMVC.Core.Diagnostics
{
    public interface IBehaviorDetailsVisitor
    {
        void ModelBinding(ModelBindingReport report);
        void FileOutput(FileOutputReport report);
        void WriteOutput(OutputReport report);
        void Redirect(RedirectReport report);
        void Exception(ExceptionReport report);
        void SetValue(SetValueReport report);
        void Authorization(AuthorizationReport report);
        void CustomTable(DataTable report);
    	void HttpStatus(HttpStatusReport report);
    }
}