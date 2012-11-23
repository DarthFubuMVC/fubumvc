using FubuCore;

namespace AspNetApplication
{
    public class ReadingRouteEndpoint
    {
        public string get_routing_data_Name_Age(RouteInput input)
        {
            return "Name={0}, Age={1}".ToFormat(input.Name, input.Age);
        }
    }

    public class RouteInput
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}