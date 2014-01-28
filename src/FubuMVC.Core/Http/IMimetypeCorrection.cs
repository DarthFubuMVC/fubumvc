using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Http
{
    public interface IMimetypeCorrection
    {
        void Correct(CurrentMimeType mimeType, ICurrentHttpRequest request, BehaviorChain chain);
    }


}