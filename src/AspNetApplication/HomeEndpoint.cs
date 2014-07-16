using System;

namespace AspNetApplication
{
    public class HomeEndpoint
    {
        public string Index()
        {
            return "hello there!";
        }

        public string get_ysod()
        {
            throw new NotImplementedException("You shall not pass!");
        }
    }
}