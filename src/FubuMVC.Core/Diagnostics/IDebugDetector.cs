namespace FubuMVC.Core.Diagnostics
{
    public interface IDebugDetector
    {
        bool IsOutputWritingLatched();
        void UnlatchWriting();
        bool IsDebugCall();
    }
}