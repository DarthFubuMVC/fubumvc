using FubuCore.Util;

namespace FubuCore.Localization
{
    public class LocalizedHeader
    {
        private readonly Cache<string, string> _properties = new Cache<string, string>();

        public string Heading { get; set; }

        // I'm thinking to leave this bit of flexibility in here for later.
        // Properties might be things like the "caption" or "title," or a help
        // topic.  
        public string this[string name]
        {
            get
            {
                return _properties[name];
            }
            set
            {
                _properties[name] = value;
            }
        }
    }
}