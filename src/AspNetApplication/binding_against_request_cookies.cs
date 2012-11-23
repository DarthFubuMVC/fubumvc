
namespace AspNetApplication
{
    public class CookieBindingEndpoint
    {
        public string get_cookie_data(CookieModel input)
        {
            return input.ToString();
        }
    }

    public class CookieModel
    {
        public string Color { get; set; }
        public string Direction { get; set; }

        public override string ToString()
        {
            return string.Format("Color: {0}, Direction: {1}", Color, Direction);
        }
    }
}