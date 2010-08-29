namespace FubuCore.Localization
{
    public class StringToken
    {
        // Notice that there's no public ctor
        // We use StringToken as a Java style
        // strongly typed enumeration
        protected StringToken(string defaultText)
        {
            DefaultText = defaultText;
        }

        public string DefaultText { get; set; }

        // I'm thinking about a way that the Key
        // gets automatically assigned to the field name  
        // of the holding class 
        public string Key { get; protected set; }

        public static StringToken FromKeyString(string key)
        {
            return new StringToken(null)
            {
                DefaultText = null
            };
        }

        // ToString() on StringToken delegates to a 
        // static Localizer class to look up the 
        // localized text for this StringToken.
        // Putting the lookup in ToString() made
        // our code shring quite a bit.
        public override string ToString()
        {
            return Localizer.TextFor(this);
        }
    }


    // This is the main "shim" interface for looking up
    // localized string values and header information

    // Static Facade over the localization subsystem for convenience.
    // Yes, I know, I have a kneejerk reaction to the word "static"
    // too.  More about that later
}