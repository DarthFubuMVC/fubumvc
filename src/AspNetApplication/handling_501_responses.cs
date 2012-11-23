using System;

namespace AspNetApplication
{
    public class ExceptionEndpoint
    {
        public string get_exception()
        {
            throw new ApplicationException("I did not like this");
        }
    }
}