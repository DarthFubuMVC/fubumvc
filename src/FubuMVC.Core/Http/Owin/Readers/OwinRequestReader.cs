using System.Collections.Generic;

namespace FubuMVC.Core.Http.Owin.Readers
{
    public class OwinRequestReader : IOwinRequestReader
    {
        private readonly IOwinRequestReader[] readers = new IOwinRequestReader[]
        {
            new MediaTypeReader(),
            new FormReader(), 
        };

        public void Read(IDictionary<string, object> environment)
        {
            readers.Each(x => x.Read(environment));
        }
    }
}