using FubuLocalization;

namespace FubuFastPack.Validation
{
    public class FastPackKeys : StringToken
    {
        protected FastPackKeys(string key, string defaultValue) : base(key, defaultValue)
        {
        }

        public static readonly StringToken FIELD_MUST_BE_UNIQUE = new FastPackKeys("FIELD_MUST_BE_UNIQUE", "Is already in use and must be unique");
    }
}