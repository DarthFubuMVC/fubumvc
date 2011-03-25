using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Models
{
    public interface IDebugReportModelBuilder
    {
        DebugReportModel Build(IDebugReport report);
    }
}