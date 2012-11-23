using FubuMVC.Core;

namespace AspNetApplication
{
    public class QueryStringEndpoint
    {
        public string get_querystring_data(QueryStringModel model)
        {
            return model.ToString();
        }
    }

    public class QueryStringModel
    {
        [QueryString]
        public string Color { get; set; }

        [QueryString]
        public string Direction { get; set; }

        public override string ToString()
        {
            return string.Format("Color: {0}, Direction: {1}", Color, Direction);
        }
    }
}