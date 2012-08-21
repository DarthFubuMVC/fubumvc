namespace AspNetApplication
{
    public class CompressedContentController
    {
         public string get_compressed_content(CompressedContentInput input)
         {
             return "Hello, World!";
         }
    }

    public class CompressedContentInput
    {
    }
}