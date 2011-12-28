namespace FubuMVC.OwinHost
{
    public interface IOwinRequestReader
    {
        void Read(byte[] bytes, int offset, int count);
        void Finish();
    }
}