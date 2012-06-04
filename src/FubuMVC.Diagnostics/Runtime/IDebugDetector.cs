namespace FubuMVC.Diagnostics.Runtime
{
    public interface IDebugDetector
    {
        bool IsOutputWritingLatched();
        void UnlatchWriting();
        bool IsDebugCall();
    }
}