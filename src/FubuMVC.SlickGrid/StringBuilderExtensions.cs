using System.Text;

namespace FubuMVC.SlickGrid
{
    public static class StringBuilderExtensions
    {
        public static void WriteJsonProp(this StringBuilder builder, object key, object value)
        {
            builder.Append(key);
            builder.Append(": ");

            if (value is string)
            {
                builder.Append("\"");
                builder.Append(value as string);
                builder.Append("\"");
            }
            else if (value is bool)
            {
                builder.Append(value.ToString().ToLower());
            }
            else
            {
                builder.Append(value.ToString());
            }

            
        }
    }
}