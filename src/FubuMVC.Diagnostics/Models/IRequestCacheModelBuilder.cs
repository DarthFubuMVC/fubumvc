using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Models
{
    public interface IRequestCacheModelBuilder
    {
        RequestCacheModel Build();
    }
}