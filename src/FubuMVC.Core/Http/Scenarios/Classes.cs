using System.Net;

namespace FubuMVC.Core.Http.Scenarios
{
    // TODO -- flush out a lot more of this
    public interface IScenarioResponse
    {
        string Text();
        HttpStatusCode StatusCode { get; }
        string StatusDescription { get; }


    }
}