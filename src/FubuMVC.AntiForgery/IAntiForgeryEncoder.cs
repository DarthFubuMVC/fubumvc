namespace FubuMVC.AntiForgery
{
    public interface IAntiForgeryEncoder
    {
        byte[] Decode(string value);
        string Encode(byte[] bytes);
    }
}